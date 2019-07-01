﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Dicom;

using DicomTypeTranslation.Converters;
using DicomTypeTranslation.Helpers;

using FAnsi.Discovery;
using FAnsi.Discovery.TypeTranslation;

using JetBrains.Annotations;

using Newtonsoft.Json;


namespace DicomTypeTranslation
{
    /// <summary>
    /// Helper methods for interacting with <see cref="DicomDataset"/> that don't involve reading/writing
    /// </summary>
    public static class DicomTypeTranslater
    {
        private static JsonConverter _defaultJsonDicomConverter = new SmiLazyJsonDicomConverter();

        /// <summary>
        /// Value Representations which are ignored when reading and writing Bson documents
        /// </summary>
        public static readonly DicomVR[] DicomBsonVrBlacklist =
        {
            DicomVR.OW,
            DicomVR.OB,
            DicomVR.UN
        };

        /// <summary>
        /// Serialize a <see cref="DicomDataset"/> to a json <see cref="string"/>.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="converter"></param>
        /// <returns>Json serialized string</returns>
        public static string SerializeDatasetToJson(DicomDataset dataset, JsonConverter converter = null)
        {
            if (dataset == null)
                throw new ArgumentNullException(nameof(dataset));

            if (converter == null)
                converter = _defaultJsonDicomConverter;

            return JsonConvert.SerializeObject(dataset, Formatting.None, converter);
        }

        /// <summary>
        /// Deserialize a json <see cref="string"/> to a <see cref="DicomDataset"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="converter"></param>
        /// <returns>Dataset</returns>
        public static DicomDataset DeserializeJsonToDataset(string json, JsonConverter converter = null)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException(nameof(json));

            if (converter == null)
                converter = _defaultJsonDicomConverter;

            return JsonConvert.DeserializeObject<DicomDataset>(json, converter);
        }

        /// <summary>
        /// Set the default json conversion method 
        /// </summary>
        /// <param name="lazy"></param>
        [UsedImplicitly]
        public static void SetLazyConversion(bool lazy)
        {
            if (lazy)
                _defaultJsonDicomConverter = new SmiLazyJsonDicomConverter();
            else
                _defaultJsonDicomConverter = new SmiStrictJsonDicomConverter();
        }

        /// <summary>
        /// Returns the supplied value unchanged unless it is an Array or an IDictionary in which case it will be converted into a string representation (including support
        /// for sub arrays / trees within arrays and vice versa).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Flatten(object value)
        {
            if (value is Array)
                return ArrayHelperMethods.GetStringRepresentation((Array)value).Trim();

            if (value is IDictionary)
                return DictionaryHelperMethods.AsciiArt((IDictionary)value).Trim();

            if (value is string)
                return ((string)value).Trim();

            return value;
        }

        #region VR Types to C# / Database types
        /// <summary>
        /// Returns a <see cref="DatabaseTypeRequest"/> for describing the datatype and length that should be used to represent the given dicom representation.
        /// If <paramref name="valueMultiplicity"/> allows multiple elements then string max is returned.
        /// </summary>
        /// <param name="valueRepresentations"></param>
        /// <param name="valueMultiplicity"></param>
        /// <returns></returns>
        public static DatabaseTypeRequest GetNaturalTypeForVr(DicomVR[] valueRepresentations, DicomVM valueMultiplicity)
        {
            //maximum lengths are defined by http://dicom.nema.org/dicom/2013/output/chtml/part05/sect_6.2.html  in bytes... lets err on the side of caution and say it is not unicode so 1 byte = 1 character
            List<DatabaseTypeRequest> vrs = valueRepresentations
                .Select(dicomVr => GetNaturalTypeForVr(dicomVr, valueMultiplicity))
                .ToList();

            return vrs.Count == 1 ? vrs[0] : Conflate(vrs);
        }

        private static DatabaseTypeRequest Conflate(List<DatabaseTypeRequest> vrs)
        {
            var toReturn = vrs.First();

            foreach (DatabaseTypeRequest newType in vrs)
            {
                Type t = toReturn.CSharpType;

                if (toReturn.CSharpType != newType.CSharpType)
                    if (
                        (toReturn.CSharpType == typeof(UInt16) || toReturn.CSharpType == typeof(Int16)) //some tags e.g. SmallestValidPixelValue can be either ushort or short 
                        && newType.CSharpType == typeof(UInt16) || newType.CSharpType == typeof(Int16))
                        t = typeof(Int32); //if they are being all creepy about it just use an int that way theres definetly enough space
                    else
                        throw new Exception("Incompatible Types '" + toReturn.CSharpType + "' and '" + newType.CSharpType + "'");

                toReturn = new DatabaseTypeRequest(
                    t,
                    Conflate(toReturn.MaxWidthForStrings, newType.MaxWidthForStrings),
                    DecimalSize.Combine(toReturn.DecimalPlacesBeforeAndAfter, newType.DecimalPlacesBeforeAndAfter));
            }

            return toReturn;
        }

        private static int? Conflate(int? a, int? b)
        {
            if (a == null && b == null)
                return null;

            if (a == null)
                return b;

            if (b == null)
                return a;

            return Math.Max(a.Value, b.Value);
        }

        /// <inheritdoc cref="GetNaturalTypeForVr(DicomVR[], DicomVM)"/>
        public static DatabaseTypeRequest GetNaturalTypeForVr(DicomVR dicomVr, DicomVM valueMultiplicity)
        {
            var decimalSize = new DecimalSize(19, 19);

            //if it's an array just use a big string to represent it
            if (valueMultiplicity.Maximum > 1)
                return new DatabaseTypeRequest(typeof(string), int.MaxValue);

            if (dicomVr == DicomVR.AE)
                return new DatabaseTypeRequest(typeof(string), 16);

            if (dicomVr == DicomVR.AS)
                return new DatabaseTypeRequest(typeof(string), 4);

            //Attribute Tag (DicomItem)
            if (dicomVr == DicomVR.AT)
                return new DatabaseTypeRequest(typeof(string), 4);

            if (dicomVr == DicomVR.CS)
                return new DatabaseTypeRequest(typeof(string), 16);

            if (dicomVr == DicomVR.DA)
                return new DatabaseTypeRequest(typeof(DateTime));

            if (dicomVr == DicomVR.DS)
                return new DatabaseTypeRequest(typeof(decimal), null, decimalSize); //16 bytes maximum but representation is a string...

            if (dicomVr == DicomVR.DT)
                return new DatabaseTypeRequest(typeof(DateTime));

            if (dicomVr == DicomVR.FD)
                return new DatabaseTypeRequest(typeof(double), null, decimalSize);

            if (dicomVr == DicomVR.FL)
                return new DatabaseTypeRequest(typeof(float), null, decimalSize);

            if (dicomVr == DicomVR.IS)
                return new DatabaseTypeRequest(typeof(int), null);

            if (dicomVr == DicomVR.LO)
                return new DatabaseTypeRequest(typeof(string), 64);

            if (dicomVr == DicomVR.LT)
                return new DatabaseTypeRequest(typeof(string), 10240);

            if (dicomVr == DicomVR.OB)
                return new DatabaseTypeRequest(typeof(byte[]));

            if (dicomVr == DicomVR.OD)
                return new DatabaseTypeRequest(typeof(double));

            if (dicomVr == DicomVR.OF)
                return new DatabaseTypeRequest(typeof(float), null, decimalSize);

            if (dicomVr == DicomVR.OL)
                return new DatabaseTypeRequest(typeof(uint));

            if (dicomVr == DicomVR.OW)
                return new DatabaseTypeRequest(typeof(ushort));

            if (dicomVr == DicomVR.PN)
                return new DatabaseTypeRequest(typeof(string), 320); //64 characters but up to 5?

            if (dicomVr == DicomVR.SH)
                return new DatabaseTypeRequest(typeof(string), 16);

            if (dicomVr == DicomVR.SL)
                return new DatabaseTypeRequest(typeof(int));

            if (dicomVr == DicomVR.SQ)
                return new DatabaseTypeRequest(typeof(string), int.MaxValue);

            if (dicomVr == DicomVR.SS)
                return new DatabaseTypeRequest(typeof(short));

            if (dicomVr == DicomVR.ST)
                return new DatabaseTypeRequest(typeof(string), int.MaxValue);

            if (dicomVr == DicomVR.TM)
                return new DatabaseTypeRequest(typeof(TimeSpan));

            if (dicomVr == DicomVR.UC) //unlimited characters
                return new DatabaseTypeRequest(typeof(string), 10240); //will be varchar max etc anyway

            if (dicomVr == DicomVR.UI)
                return new DatabaseTypeRequest(typeof(string), 64);

            if (dicomVr == DicomVR.UL)
                return new DatabaseTypeRequest(typeof(uint));

            if (dicomVr == DicomVR.UN)
                return new DatabaseTypeRequest(typeof(string), 10240);

            if (dicomVr == DicomVR.UR)
                return new DatabaseTypeRequest(typeof(string), int.MaxValue);//url

            if (dicomVr == DicomVR.US)
                return new DatabaseTypeRequest(typeof(ushort));

            if (dicomVr == DicomVR.UT)
                return new DatabaseTypeRequest(typeof(string), 10240);

            throw new ArgumentOutOfRangeException("Invalid value representation:" + dicomVr);
        }

        #endregion
    }
}
