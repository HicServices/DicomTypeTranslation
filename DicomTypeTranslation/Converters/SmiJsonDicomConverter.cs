﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dicom;
using Dicom.IO.Buffer;
using Newtonsoft.Json;

namespace DicomTypeTranslation.Converters
{
    /// <summary>
    /// SMI JSON to DICOM converter. Lazily converts between certain DICOM value types to allow greater coverage over real data.
    /// This means it does not comply with the formal DICOM JSON specification.
    /// </summary>
    public class SmiJsonDicomConverter : JsonConverter
    {
        private const string VALUE_PROPERTY_NAME = "val";
        private const string INL_BIN_PROPERTY_NAME = "bin";
        private const string BLK_URI_PROPERTY_NAME = "uri";
        private const string VR_PROPERTY_NAME = "vr";


        static SmiJsonDicomConverter()
        {
#pragma warning disable CS0618 // Obsolete
            DicomValidation.AutoValidation = false;
#pragma warning restore CS0618
        }

        /// <summary>
        /// Constructor with 1 bool argument required for compatibility testing with
        /// original version of the class.
        /// </summary>
        /// <param name="_"></param>
        // ReSharper disable once UnusedParameter.Local
        public SmiJsonDicomConverter(bool _ = false) { }

        #region JsonConverter overrides

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var dataset = (DicomDataset)value;

            writer.WriteStartObject();

            foreach (DicomItem item in dataset)
            {
                // Group length (gggg,0000) attributes shall not be included in a DICOM JSON Model object.
                if (((uint)item.Tag & 0xffff) == 0)
                    continue;

                writer.WritePropertyName(item.Tag.Group.ToString("X4") + item.Tag.Element.ToString("X4"));
                WriteJsonDicomItem(writer, item, serializer);
            }

            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dataset = new DicomDataset();

            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonReaderException("Malformed DICOM json");

            reader.Read();

            while (reader.TokenType == JsonToken.PropertyName)
            {
                DicomTag tag = ParseTag((string)reader.Value);

                reader.Read();

                DicomItem item = ReadJsonDicomItem(tag, reader, serializer);

                dataset.Add(item);

                reader.Read();
            }

            // Ensure all private tags have a reference to their Private Creator tag
            foreach (DicomItem item in dataset)
            {
                if (item.Tag.IsPrivate && ((item.Tag.Element & 0xff00) != 0))
                {
                    var privateCreatorTag = new DicomTag(item.Tag.Group, (ushort)(item.Tag.Element >> 8));

                    if (dataset.Contains(privateCreatorTag))
                        item.Tag.PrivateCreator = new DicomPrivateCreator(dataset.GetSingleValue<string>(privateCreatorTag));
                }
            }

            if (reader.TokenType != JsonToken.EndObject)
                throw new JsonReaderException("Malformed DICOM json");

            return dataset;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(DicomDataset).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        #endregion

        #region WriteJson helpers

        private void WriteJsonDicomItem(JsonWriter writer, DicomItem item, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(VR_PROPERTY_NAME);
            writer.WriteValue(item.ValueRepresentation.Code);

            // Could also do if(item is DicomStringElement) {...} here, but better to be explicit
            // in specifying each VR and also to match the old version of the converter
            switch (item.ValueRepresentation.Code)
            {
                // Subclasses of DicomStringElement

                case "AE": // Application Entity
                case "AS": // Age String
                case "CS": // Code String
                case "DS": // Decimal String
                case "IS": // Integer String
                case "LO": // Long String
                case "PN": // Person Name
                case "SH": // Short String
                case "UI": // Unique Identifier
                case "UC": // Unlimited Characters
                case "LT": // Long Text
                case "ST": // Short Text
                case "UR": // Universal Resource
                case "UT": // Unlimited Text
                case "DA": // Date
                case "DT": // DateTime
                case "TM": // Time
                    WriteJsonStringElement(writer, (DicomElement)item);
                    break;

                // Subclasses of DicomValueElement<T>

                // DicomValueElement<double>
                case "FD": // Floating Point Double
                    WriteDicomValueElement<double>(writer, (DicomElement)item);
                    break;

                // DicomValueElement<float>
                case "FL": // Floating Point Single
                    WriteDicomValueElement<float>(writer, (DicomElement)item);
                    break;

                // DicomValueElement<int>
                case "SL": // Signed Long
                    WriteDicomValueElement<int>(writer, (DicomElement)item);
                    break;

                // DicomValueElement<uint>
                case "UL": // Unsigned Long
                    WriteDicomValueElement<uint>(writer, (DicomElement)item);
                    break;

                // DicomValueElement<short>
                case "SS": // Signed Short
                    WriteDicomValueElement<short>(writer, (DicomElement)item);
                    break;

                case "SV": // Signed 64-bit Very Long
                    WriteDicomValueElement<long>(writer, (DicomElement)item);
                    break;

                // DicomValueElement<ushort>
                case "US": // Unsigned Short
                    WriteDicomValueElement<ushort>(writer, (DicomElement)item);
                    break;

                case "UV": // Unsigned 64-bit Very Long
                    WriteDicomValueElement<ulong>(writer, (DicomElement)item);
                    break;

                // Other element types

                case "AT": // Attribute Tag
                    WriteJsonAttributeTag(writer, (DicomElement)item);
                    break;

                case "SQ": // Sequence
                    WriteJsonSequence(writer, (DicomSequence)item, serializer);
                    break;

                case "OB":
                case "OD":
                case "OF":
                case "OL":
                case "OV":
                case "OW":
                case "UN":
                    WriteJsonOther(writer, item);
                    break;

                default:
                    throw new ArgumentException("No case implemented to write data for VR " + item.ValueRepresentation.Code + " to JSON");
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Write any subclass of <see cref="DicomStringElement"/> to json
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elem"></param>
        private static void WriteJsonStringElement(JsonWriter writer, DicomElement elem)
        {
            if (elem.Count == 0)
                return;

            writer.WritePropertyName(VALUE_PROPERTY_NAME);

            string val = elem.Get<string>().TrimEnd('\0');
            writer.WriteValue(val);
        }

        private static void WriteDicomValueElement<T>(JsonWriter writer, DicomElement elem) where T : struct
        {
            if (elem.Count == 0)
                return;

            writer.WritePropertyName(VALUE_PROPERTY_NAME);
            writer.WriteStartArray();

            foreach (T val in elem.Get<T[]>())
                writer.WriteValue(val);

            writer.WriteEndArray();
        }

        private static void WriteJsonAttributeTag(JsonWriter writer, DicomElement elem)
        {
            if (elem.Count == 0)
                return;

            writer.WritePropertyName(VALUE_PROPERTY_NAME);

            var sb = new StringBuilder();

            foreach (DicomTag val in elem.Get<DicomTag[]>())
                sb.Append(((uint)val).ToString("X8"));

            if (sb.Length % 8 != 0)
                throw new JsonException("AttributeTag string of length " + sb.Length + " is not divisible by 8");

            writer.WriteValue(sb.ToString());
        }
        private void WriteJsonSequence(JsonWriter writer, DicomSequence seq, JsonSerializer serializer)
        {
            if (seq.Items.Count == 0)
                return;

            writer.WritePropertyName(VALUE_PROPERTY_NAME);
            writer.WriteStartArray();

            foreach (DicomDataset child in seq.Items)
                WriteJson(writer, child, serializer);

            writer.WriteEndArray();
        }

        private static void WriteJsonOther(JsonWriter writer, DicomItem item)
        {
            // DicomFragmentSequence - Only for pixel data
            if (!(item is DicomElement))
                return;

            var elem = (DicomElement)item;

            if (!DicomTypeTranslater.SerializeBinaryData && DicomTypeTranslater.DicomVrBlacklist.Contains(elem.ValueRepresentation))
                return;

            if (elem.Buffer is IBulkDataUriByteBuffer buffer)
            {
                writer.WritePropertyName(BLK_URI_PROPERTY_NAME);
                writer.WriteValue(buffer.BulkDataUri);
            }
            else if (elem.Count != 0)
            {
                writer.WritePropertyName(INL_BIN_PROPERTY_NAME);
                writer.WriteValue(Convert.ToBase64String(elem.Buffer.Data));
            }
        }

        #endregion

        #region ReadJson helpers

        private static DicomTag ParseTag(string tagStr)
        {
            ushort group = Convert.ToUInt16(tagStr.Substring(0, 4), 16);
            ushort element = Convert.ToUInt16(tagStr.Substring(4), 16);
            var tag = new DicomTag(group, element);
            return tag;
        }

        private DicomItem ReadJsonDicomItem(DicomTag tag, JsonReader reader, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonReaderException("Malformed DICOM json");

            reader.Read();

            if (reader.TokenType != JsonToken.PropertyName)
                throw new JsonReaderException("Malformed DICOM json");

            if ((string)reader.Value != VR_PROPERTY_NAME)
                throw new JsonReaderException("Malformed DICOM json");

            reader.Read();

            if (reader.TokenType != JsonToken.String)
                throw new JsonReaderException("Malformed DICOM json");

            var vr = (string)reader.Value;

            if (vr.Length != 2)
                throw new JsonReaderException("Malformed DICOM json");

            object data;

            switch (vr)
            {
                // Subclasses of DicomStringElement

                case "AE": // Application Entity
                case "AS": // Age String
                case "CS": // Code String
                case "DS": // Decimal String
                case "IS": // Integer String
                case "LO": // Long String
                case "PN": // Person Name
                case "SH": // Short String
                case "UI": // Unique Identifier
                case "UC": // Unlimited Characters
                case "LT": // Long Text
                case "ST": // Short Text
                case "UR": // Universal Resource
                case "UT": // Unlimited Text
                case "DA": // Date
                case "DT": // DateTime
                case "TM": // Time
                case "AT": // Attribute Tag
                    data = ReadJsonString(reader);
                    break;

                // Subclasses of DicomValueElement<T>

                // DicomValueElement<double>
                case "FD": // Floating Point Double
                    data = ReadJsonNumeric<double>(reader);
                    break;

                // DicomValueElement<float>
                case "FL": // Floating Point Single
                    data = ReadJsonNumeric<float>(reader);
                    break;

                // DicomValueElement<int>
                case "SL": // Signed Long
                    data = ReadJsonNumeric<int>(reader);
                    break;

                // DicomValueElement<uint>
                case "UL": // Unsigned Long
                    data = ReadJsonNumeric<uint>(reader);
                    break;

                // DicomValueElement<short>
                case "SS": // Signed Short
                    data = ReadJsonNumeric<short>(reader);
                    break;

                case "SV":
                    data = ReadJsonNumeric<long>(reader);
                    break;

                // DicomValueElement<ushort>
                case "US": // Unsigned Short
                    data = ReadJsonNumeric<ushort>(reader);
                    break;

                case "UV":
                    data = ReadJsonNumeric<ulong>(reader);
                    break;

                // Sequence
                case "SQ":
                    data = ReadJsonSequence(reader, serializer);
                    break;

                // OtherX elements
                case "OB":
                case "OD":
                case "OF":
                case "OL":
                case "OW":
                case "OV":
                case "UN":
                    data = ReadJsonOX(reader);
                    break;

                default:
                    throw new ArgumentException("No case implemented to read object data for VR " + vr + " from JSON");
            }

            if (reader.TokenType != JsonToken.EndObject)
                throw new JsonReaderException("Malformed DICOM json");

            return CreateDicomItem(tag, vr, data);
        }

        private static DicomItem CreateDicomItem(DicomTag tag, string vr, object data)
        {
            switch (vr)
            {
                case "AE":
                    return new DicomApplicationEntity(tag, (string)data);
                case "AS":
                    return new DicomAgeString(tag, (string)data);
                case "AT":
                    return CreateDicomAttributeTag(tag, (string)data);
                case "CS":
                    return new DicomCodeString(tag, (string)data);
                case "DA":
                    return new DicomDate(tag, (string)data);
                case "DS":
                    return new DicomDecimalString(tag, (string)data);
                case "DT":
                    return new DicomDateTime(tag, (string)data);
                case "FD":
                    return new DicomFloatingPointDouble(tag, (double[])data);
                case "FL":
                    return new DicomFloatingPointSingle(tag, (float[])data);
                case "IS":
                    return new DicomIntegerString(tag, (string)data);
                case "LO":
                    return new DicomLongString(tag, Encoding.UTF8, (string)data);
                case "LT":
                    return new DicomLongText(tag, Encoding.UTF8, (string)data);
                case "OB":
                    return new DicomOtherByte(tag, (IByteBuffer)data);
                case "OD":
                    return new DicomOtherDouble(tag, (IByteBuffer)data);
                case "OF":
                    return new DicomOtherFloat(tag, (IByteBuffer)data);
                case "OL":
                    return new DicomOtherLong(tag, (IByteBuffer)data);
                case "OW":
                    return new DicomOtherWord(tag, (IByteBuffer)data);
                case "OV":
                    return new DicomOtherVeryLong(tag, (IByteBuffer)data);
                case "PN":
                    return new DicomPersonName(tag, Encoding.UTF8, (string)data);
                case "SH":
                    return new DicomShortString(tag, Encoding.UTF8, (string)data);
                case "SL":
                    return new DicomSignedLong(tag, (int[])data);
                case "SQ":
                    return new DicomSequence(tag, (DicomDataset[])data);
                case "SS":
                    return new DicomSignedShort(tag, (short[])data);
                case "ST":
                    return new DicomShortText(tag, Encoding.UTF8, (string)data);
                case "SV":
                    return new DicomSignedVeryLong(tag, (long[])data);
                case "TM":
                    return new DicomTime(tag, (string)data);
                case "UC":
                    return new DicomUnlimitedCharacters(tag, Encoding.UTF8, (string)data);
                case "UI":
                    return new DicomUniqueIdentifier(tag, (string)data);
                case "UL":
                    return new DicomUnsignedLong(tag, (uint[])data);
                case "UN":
                    return new DicomUnknown(tag, (IByteBuffer)data);
                case "UR":
                    return new DicomUniversalResource(tag, Encoding.UTF8, (string)data);
                case "US":
                    return new DicomUnsignedShort(tag, (ushort[])data);
                case "UT":
                    return new DicomUnlimitedText(tag, Encoding.UTF8, (string)data);
                case "UV":
                    return new DicomUnsignedVeryLong(tag, (ulong[])data);
                default:
                    throw new ArgumentException("No method implemented to create a DicomItem of VR " + vr + " from JSON");
            }
        }

        private static string ReadJsonString(JsonReader reader)
        {
            reader.Read();

            if (reader.TokenType == JsonToken.EndObject)
                return "";

            if (reader.TokenType != JsonToken.PropertyName || (string)reader.Value != VALUE_PROPERTY_NAME)
                throw new JsonReaderException("Malformed DICOM json");

            reader.ReadAsString();

            var val = (string)reader.Value;

            reader.Read();

            return val;
        }

        private static DicomAttributeTag CreateDicomAttributeTag(DicomTag tag, string str)
        {
            if (str == "")
                return new DicomAttributeTag(tag);

            if (str.Length % 8 != 0)
                throw new JsonException("Can't parse string of length " + str.Length + " to an AttributeTag. (Needs to be divisible by 8)");

            var split = new List<string>();

            for (var i = 0; i < str.Length; i += 8)
                split.Add(str.Substring(i, 8));

            return new DicomAttributeTag(tag, split.Select(ParseTag).ToArray());
        }

        private static T[] ReadJsonNumeric<T>(JsonReader reader)
        {
            reader.Read();

            if (reader.TokenType == JsonToken.EndObject)
                return new T[0];

            if (reader.TokenType != JsonToken.PropertyName && (string)reader.Value != VALUE_PROPERTY_NAME)
                throw new JsonReaderException("Malformed DICOM json");

            reader.Read();

            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonReaderException("Malformed DICOM json");

            reader.Read();

            var values = new List<T>();

            while (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            {
                values.Add((T)Convert.ChangeType(reader.Value, typeof(T)));
                reader.Read();
            }

            if (reader.TokenType != JsonToken.EndArray)
                throw new JsonReaderException("Malformed DICOM json");

            reader.Read();

            return values.ToArray();
        }

        private static IByteBuffer ReadJsonOX(JsonReader reader)
        {
            reader.Read();

            if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == INL_BIN_PROPERTY_NAME)
            {
                return ReadJsonInlineBinary(reader);
            }

            if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == BLK_URI_PROPERTY_NAME)
            {
                return ReadJsonBulkDataUri(reader);
            }

            return EmptyBuffer.Value;
        }

        private static IByteBuffer ReadJsonInlineBinary(JsonReader reader)
        {
            reader.Read();

            if (reader.TokenType != JsonToken.String)
                throw new JsonReaderException("Malformed DICOM json");

            var data = new MemoryByteBuffer(Convert.FromBase64String(reader.Value as string));
            reader.Read();

            return data;
        }

        private static IBulkDataUriByteBuffer ReadJsonBulkDataUri(JsonReader reader)
        {
            reader.Read();

            if (reader.TokenType != JsonToken.String)
                throw new JsonReaderException("Malformed DICOM json");

            var data = new BulkDataUriByteBuffer((string)reader.Value);
            reader.Read();

            return data;
        }

        private DicomDataset[] ReadJsonSequence(JsonReader reader, JsonSerializer serializer)
        {
            reader.Read();

            if (reader.TokenType != JsonToken.PropertyName || (string)reader.Value != VALUE_PROPERTY_NAME)
                return new DicomDataset[0];

            reader.Read();

            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonReaderException("Malformed DICOM json");

            reader.Read();

            var childItems = new List<DicomDataset>();

            while (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Null)
            {
                childItems.Add((DicomDataset)ReadJson(reader, typeof(DicomDataset), null, serializer));
                reader.Read();
            }

            if (reader.TokenType != JsonToken.EndArray)
                throw new JsonReaderException("Malformed DICOM json");

            reader.Read();

            return childItems.ToArray();
        }

        #endregion
    }
}
