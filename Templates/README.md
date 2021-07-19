# Templates

- [CT](#ct-computerised-tomography)
- [MR](#mr-magnetic-resonance)
- [PT](#pt-positron-emission-tomography-PET)
- [NM](#nm-nuclear-medicine)
- [DX](#dx-digital-radiography)
- [XA](#xa-x-ray-angiography)
- [SR](#sr-structured-report)
- [OTHER](#other)

## CT (Computerised Tomography)

Schema with 

### StudyTable

| Field | Description |
| ------------- | ------------- |
| PatientID |  |
| StudyInstanceUID |  |
| StudyDate |  |
| StudyTime |  |
| ModalitiesInStudy |  |
| StudyDescription |  |
| AccessionNumber |  |
| PatientSex |  |
| PatientAge |  |
| NumberOfStudyRelatedInstances |  |
| PatientBirthDate |  |

### SeriesTable

| Field | Description |
| ------------- | ------------- |
| StudyInstanceUID |  |
| SeriesInstanceUID |  |
| Modality |  |
| InstitutionName |  |
| ProtocolName |  |
| ProcedureCodeSequence_CodeValue |  |
| PerformedProcedureStepDescription |  |
| SeriesDescription |  |
| SeriesDate |  |
| SeriesTime |  |
| BodyPartExamined |  |
| DeviceSerialNumber |  |
| SeriesNumber |  |

### ImageTable

| Field | Description |
| ------------- | ------------- |
| SeriesInstanceUID |  |
| SOPInstanceUID |  |
| BurnedInAnnotation |  |
| RelativeFileArchiveURI |  |
| MessageGuid |  |
| SliceLocation |  |
| SliceThickness |  |
| SpacingBetweenSlices |  |
| SpiralPitchFactor |  |
| KVP |  |
| ExposureTime |  |
| Exposure |  |
| ImageType |  |
| ManufacturerModelName |  |
| Manufacturer |  |
| SoftwareVersions |  |
| XRayTubeCurrent |  |
| PhotometricInterpretation |  |
| ContrastBolusRoute |  |
| ContrastBolusAgent |  |
| AcquisitionNumber |  |
| AcquisitionDate |  |
| AcquisitionTime |  |
| ImagePositionPatient |  |
| PixelSpacing |  |
| FieldOfViewDimensions |  |
| FieldOfViewDimensionsInFloat |  |
| DerivationDescription |  |
| LossyImageCompression |  |
| LossyImageCompressionMethod |  |
| LossyImageCompressionRatio |  |
| LossyImageCompressionRetired |  |
| ScanOptions |  |
| StudyInstanceUID | |
| PatientID | |
| DicomFileSize | |

## MR (Magnetic Resonance)

### StudyTable

| Field | Description |
| ------------- | ------------- |
| PatientID |  |
| StudyInstanceUID |  |
| StudyDate |  |
| StudyTime |  |
| ModalitiesInStudy |  |
| StudyDescription |  |
| AccessionNumber |  |
| PatientSex |  |
| PatientAge |  |
| PatientBirthDate |  |
| NumberOfStudyRelatedInstances |  |

### SeriesTable

| Field | Description |
| ------------- | ------------- |
| StudyInstanceUID |  |
| SeriesInstanceUID |  |
| Modality |  |
| InstitutionName |  |
| ProtocolName |  |
| ProcedureCodeSequence_CodeValue |  |
| PerformedProcedureStepDescription |  |
| SeriesDescription |  |
| SeriesDate |  |
| SeriesTime |  |
| BodyPartExamined |  |
| DeviceSerialNumber |  |
| SeriesNumber |  |
| MRAcquisitionType |  |
| AngioFlag |  |
| MagneticFieldStrength |  |
| TransmitCoilName |  |
| PatientPosition |  |

### ImageTable

| Field | Description |
| ------------- | ------------- |
| SeriesInstanceUID |  |
| SOPInstanceUID |  |
| BurnedInAnnotation |  |
| RelativeFileArchiveURI |  |
| MessageGuid |  |
| SliceLocation |  |
| SliceThickness |  |
| SpacingBetweenSlices |  |
| ImageType |  |
| ManufacturerModelName |  |
| Manufacturer |  |
| SoftwareVersions |  |
| PhotometricInterpretation |  |
| ContrastBolusRoute |  |
| ContrastBolusAgent |  |
| AcquisitionNumber |  |
| AcquisitionDate |  |
| AcquisitionTime |  |
| ImagePositionPatient |  |
| ImageOrientationPatient |  |
| PixelSpacing |  |
| FieldOfViewDimensions |  |
| FieldOfViewDimensionsInFloat |  |
| DerivationDescription |  |
| LossyImageCompression |  |
| LossyImageCompressionMethod |  |
| LossyImageCompressionRatio |  |
| LossyImageCompressionRetired |  |
| ScanOptions |  |
| EchoTime |  |
| EchoNumbers |  |
| EchoTrainLength |  |
| InversionTime |  |
| RepetitionTime |  |
| NumberOfPhaseEncodingSteps |  |
| FlipAngle |  |
| VariableFlipAngleFlag |  |
| SequenceName |  |
| StudyInstanceUID | |
| PatientID | |
| DicomFileSize | |


## PT (Positron emission tomography (PET))

What is a PET scan

-  https://youtu.be/GHLBcCv4rqk

Example Dataset
- https://wiki.cancerimagingarchive.net/display/Public/Head-Neck-PET-CT

Observations

- PET is normally combined with a CT at the same time



### StudyTable

| Field | Description |
| ------------- | ------------- |
| PatientID |  |
| StudyInstanceUID |  |
| StudyDate |  |
| StudyTime |  |
| ModalitiesInStudy |  |
| StudyDescription |  |
| AccessionNumber |  |
| PatientSex |  |
| PatientAge |  |
| NumberOfStudyRelatedInstances |  |
| PatientBirthDate |  |

### SeriesTable

| Field | Description |
| ------------- | ------------- |
| StudyInstanceUID |  |
| SeriesInstanceUID |  |
| Modality |  |
| InstitutionName |  |
| ProtocolName |  |
| ProcedureCodeSequence_CodeValue |  |
| PerformedProcedureStepDescription |  |
| SeriesDescription |  |
| SeriesDate |  |
| SeriesTime |  |
| BodyPartExamined |  |
| DeviceSerialNumber |  |
| SeriesNumber |  |
| NumberOfSlices |  |
| EnergyWindowRange_EnergyWindowLowerLimit |  |
| EnergyWindowRange_EnergyWindowUpperLimit |  |
| Radiopharmaceutical_Radiopharmaceutical |  |
| Radiopharmaceutical_RadiopharmaceuticalVolume |  |
| Radiopharmaceutical_RadionuclideCode_CodeValue |  |
| Radiopharmaceutical_RadionuclideCode_CodeMeaning |  |
| Radiopharmaceutical_RadionuclideTotalDose |  |
| RandomsCorrectionMethod |  |
| AttenuationCorrectionMethod |  |
| DecayCorrection |  |
| ReconstructionMethod |  |
| ScatterCorrectionMethod |  |
| DateOfLastCalibration |  |
|TimeOfLastCalibration | |

### ImageTable

| Field | Description |
| ------------- | ------------- |
| SeriesInstanceUID |  |
| SOPInstanceUID |  |
| BurnedInAnnotation |  |
| RelativeFileArchiveURI |  |
| MessageGuid |  |
| SliceLocation |  |
| SliceThickness |  |
| SpacingBetweenSlices |  |
| ImageType |  |
| ManufacturerModelName |  |
| Manufacturer |  |
| SoftwareVersions |  |
| PhotometricInterpretation |  |
| AcquisitionNumber |  |
| AcquisitionDate |  |
| AcquisitionTime |  |
| ImagePositionPatient |  |
| PixelSpacing |  |
| FieldOfViewDimensions |  |
| FieldOfViewDimensionsInFloat |  |
| DerivationDescription |  |
| LossyImageCompression |  |
| LossyImageCompressionMethod |  |
| LossyImageCompressionRatio |  |
| LossyImageCompressionRetired |  |
| ScanOptions |  |
| ActualFrameDuration | |
| StudyInstanceUID | |
| PatientID | |
| DicomFileSize | |



## NM (Nuclear Medicine)

This modality contains both SPECT and other forms of nuclear medicine.

### StudyTable

| Field | Description |
| ------------- | ------------- |
| PatientID |  |
| StudyInstanceUID |  |
| StudyDate |  |
| StudyTime |  |
| ModalitiesInStudy |  |
| StudyDescription |  |
| AccessionNumber |  |
| PatientSex |  |
| PatientAge |  |
| NumberOfStudyRelatedInstances |  |
| PatientBirthDate |  |

### SeriesTable

| Field | Description |
| ------------- | ------------- |
| StudyInstanceUID |  |
| SeriesInstanceUID |  |
| Modality |  |
| InstitutionName |  |
| ProtocolName |  |
| ProcedureCodeSequence_CodeValue |  |
| PerformedProcedureStepDescription |  |
| SeriesDescription |  |
| SeriesDate |  |
| SeriesTime |  |
| BodyPartExamined |  |
| DeviceSerialNumber |  |
| SeriesNumber |  |
| NumberOfSlices |  |
| EnergyWindowRange_EnergyWindowLowerLimit |  |
| EnergyWindowRange_EnergyWindowUpperLimit |  |
| Radiopharmaceutical_Radiopharmaceutical |  |
| Radiopharmaceutical_RadiopharmaceuticalVolume |  |
| Radiopharmaceutical_RadionuclideCode_CodeValue |  |
| Radiopharmaceutical_RadionuclideCode_CodeMeaning |  |
| Radiopharmaceutical_RadionuclideTotalDose |  |
|NumberOfDetectors                                         | |
|DetectorInformation_CollimatorGridName            | |
|DetectorInformation_CollimatorType                | |
|DetectorInformation_FieldOfViewShape              | |
|DetectorInformation_FieldOfViewDimensions         | |
|DetectorInformation_ZoomFactor                    | |
|DetectorInformation_RadialPosition                | |
|RotationInformation_ScanArc                       | |
|RotationInformation_AngularStep                   | |
|RotationInformation_NumberOfFramesInRotation      | |
|TypeOfDetectorMotion                                      | |
| RandomsCorrectionMethod |  |
| AttenuationCorrectionMethod |  |
| DecayCorrection |  |
| ReconstructionMethod |  |
| ScatterCorrectionMethod |  |
| DateOfLastCalibration |  |
|TimeOfLastCalibration | |

### ImageTable

| Field | Description |
| ------------- | ------------- |
| SeriesInstanceUID |  |
| SOPInstanceUID |  |
| BurnedInAnnotation |  |
| RelativeFileArchiveURI |  |
| MessageGuid |  |
| SliceLocation |  |
| SliceThickness |  |
| SpacingBetweenSlices |  |
| ImageType |  |
| ManufacturerModelName |  |
| Manufacturer |  |
| SoftwareVersions |  |
| PhotometricInterpretation |  |
| AcquisitionNumber |  |
| AcquisitionDate |  |
| AcquisitionTime |  |
| ImagePositionPatient |  |
| PixelSpacing |  |
| FieldOfViewDimensions |  |
| FieldOfViewDimensionsInFloat |  |
| DerivationDescription |  |
| LossyImageCompression |  |
| LossyImageCompressionMethod |  |
| LossyImageCompressionRatio |  |
| LossyImageCompressionRetired |  |
| ScanOptions |  |
| ActualFrameDuration | |
| StudyInstanceUID | |
| PatientID | |
| DicomFileSize | |


## DX Digital Radiography

For a great introduction to medical imaging and specifically X-Rays check out:

https://www.youtube.com/watch?v=cTwbSDcCN4E

### StudyTable


| Field | Description |
| ------------- | ------------- |
|PatientID| |
|StudyInstanceUID| |
|StudyDate| |
|StudyTime| |
|ModalitiesInStudy| |
|StudyDescription| |
|AccessionNumber| |
|PatientSex| |
|PatientAge| |
|NumberOfStudyRelatedInstances| |
|PatientBirthDate| |

### SeriesTable


| Field | Description |
| ------------- | ------------- |
|StudyInstanceUID| |
|SeriesInstanceUID| |
|Modality| |
|InstitutionName| |
|ProtocolName| |
|ProcedureCodeSequence_CodeValue| |
|PerformedProcedureStepDescription| |
|SeriesDescription| |
|SeriesDate| |
|SeriesTime| |
|BodyPartExamined| |
|DeviceSerialNumber| |
|SeriesNumber| |

### ImageTable

| Field | Description |
| ------------- | ------------- |
|SeriesInstanceUID| |
|SOPInstanceUID| |
|BurnedInAnnotation| |
|RelativeFileArchiveURI| |
|MessageGuid| |
|KVP| |
|ExposureTime| |
|Exposure| |
|ImageType| |
|ManufacturerModelName| |
|Manufacturer| |
|SoftwareVersions| |
|XRayTubeCurrent| |
|PhotometricInterpretation| |
|AcquisitionNumber| |
|AcquisitionDate| |
|AcquisitionTime| |
|ImagePositionPatient| |
|PixelSpacing| |
|DerivationDescription| |
|LossyImageCompression| |
|LossyImageCompressionMethod| |
|LossyImageCompressionRatio| |
|LossyImageCompressionRetired| |
|ScanOptions| |
|SpatialResolution| |
|DistanceSourceToDetector| |
|ExposureInuAs| |
|DistanceSourceToPatient| |
|EstimatedRadiographicMagnificationFactor| |
|ImageAndFluoroscopyAreaDoseProduct| |
|Grid| |
|Rows| |
|Columns| |
|LongitudinalTemporalInformationModified| |
|PixelIntensityRelationshipSign| |
|WindowCenter| |
|WindowWidth| |
|RescaleIntercept| |
|RescaleSlope| |
|RescaleType| |
|CollimatorShape| |
|CollimatorLeftVerticalEdge| |
|CollimatorRightVerticalEdge| |
|CollimatorUpperHorizontalEdge| |
|CollimatorLowerHorizontalEdge| |
|ViewPosition| |
|DetectorTemperature| |
|DetectorType| |
|DetectorMode| |
|DetectorID| |
|PositionerType| |
|RelativeXRayExposure| |
|AcquisitionDeviceProcessingDescription| |
| StudyInstanceUID | |
| PatientID | |
| DicomFileSize | |



## XA (X-ray Angiography)

### StudyTable


| Field | Description |
| ------------- | ------------- |
|StudyInstanceUID| |
|AccessionNumber| |
|ModalitiesInStudy| |
|NumberOfStudyRelatedInstances| |
|PatientAge| |
|PatientBirthDate| |
|PatientID| |
|PatientSex| |
|StudyDate| |
|StudyDescription| |
|StudyTime || 

### SeriesTable

| Field | Description |
| ------------- | ------------- |
|StudyInstanceUID| |
|SeriesInstanceUID| |
|BodyPartExamined| |
|DeviceSerialNumber| |
|InstitutionName| |
|Modality| |
|PerformedProcedureStepDescription| |
|ProcedureCodeSequence_CodeValue| |
|ProtocolName| |
|SeriesDate| |
|SeriesDescription| |
|SeriesNumber| |
|SeriesTime| |

### ImageTable

| Field | Description |
| ------------- | ------------- |
|SeriesInstanceUID| |
|SOPInstanceUID| |
|RelativeFileArchiveURI| |
|MessageGuid| |
|AcquisitionDate| |
|AcquisitionDeviceProcessingDescription| |
|AcquisitionNumber| |
|AcquisitionTime| |
|AveragePulseWidth| |
|BurnedInAnnotation| |
|CollimatorLeftVerticalEdge| |
|CollimatorLowerHorizontalEdge| |
|CollimatorRightVerticalEdge| |
|CollimatorShape| |
|CollimatorUpperHorizontalEdge| |
|ColorSpace| |
|Columns| |
|DerivationDescription| |
|DetectorConfiguration| |
|DetectorDescription| |
|DetectorID| |
|DetectorMode| |
|DetectorPrimaryAngle| |
|DetectorSecondaryAngle| |
|DetectorTemperature| |
|DetectorType| |
|DistanceSourceToDetector| |
|DistanceSourceToPatient| |
|EstimatedRadiographicMagnificationFactor| |
|Exposure| |
|ExposureInuAs| |
|ExposureTime| |
|FocalSpots| |
|Grid| |
|ImageAndFluoroscopyAreaDoseProduct| |
|ImagePositionPatient| |
|ImageType| |
|ImagerPixelSpacing| |
|IntensifierSize| |
|KVP| |
|LongitudinalTemporalInformationModified| |
|LossyImageCompression| |
|LossyImageCompressionMethod| |
|LossyImageCompressionRatio| |
|Manufacturer| |
|ManufacturerModelName| |
|PhotometricInterpretation| |
|PixelAspectRatio| |
|PixelIntensityRelationshipSign| |
|PixelSpacing| |
|PixelSpacingCalibrationDescription| |
|PixelSpacingCalibrationType| |
|PlanarConfiguration| |
|PositionerMotion| |
|PositionerType| |
|RadiationMode| |
|RadiationSetting| |
|RelativeXRayExposure| |
|RescaleIntercept| |
|RescaleSlope| |
|RescaleType| |
|Rows| |
|SamplesPerPixel| |
|ScanOptions| |
|SoftwareVersions| |
|SpatialResolution| |
|TypeOfFilters| |
|WindowCenter| |
|WindowWidth| |
|XRayTubeCurrent| |
|XRayTubeCurrentInuA| |
| StudyInstanceUID | |
| PatientID | |
| DicomFileSize | |


## SR (Structured Report)

Structured Report dicom files contain information about a study or series.  It can include radiologists notes, diagnoses etc.

Since structured reports are almost exclusively single file entities only an ImageTable schema is defined (No study/series/image division of tags).

### ImageTable


| Field | Description |
| ------------- | ------------- |
|PatientID                        |  |
|StudyDate                        |  |
|StudyTime                        |  |
|ModalitiesInStudy                |  |
|StudyDescription                 |  |
|AccessionNumber                  |  |
|PatientSex                       |  |
|PatientAge                       |  |
|NumberOfStudyRelatedInstances    |  |
|PatientBirthDate                 |  |
|StudyInstanceUID                 |  |
|SeriesInstanceUID                |  |
|SOPInstanceUID                   |  |
|Modality                         |  |
|InstitutionName                  |  |
|SeriesDescription                |  |
|SeriesDate                       |  |
|SeriesTime                       |  |
|DeviceSerialNumber               |  |
|SeriesNumber                     |  |
|BurnedInAnnotation               |  |
|ImageType                        |  |
|ManufacturerModelName            |  |
|Manufacturer                     |  |
|SoftwareVersions                 |  |
|CompletionFlag                   |  |
|VerificationFlag                 |  |
|StrainNomenclature               |  |
|StrainDescription                |  |
|SOPClassUID                  |  |
| Study_ReferencedSOPClassUID |  |
| Study_ReferencedSOPInstanceUID |  |
| RefImageSeq_ReferencedSOPClassUID |  |
| RefImageSeq_ReferencedSOPInstanceUID |  |
| SourceImageSeq_ReferencedSOPClassUID |  |
| SourceImageSeq_ReferencedSOPInstanceUID |  |
| RefProcStep_ReferencedSOPClassUID |  |
| RefProcStep_ReferencedSOPInstanceUID |  |
| CurrentReqEvidence_StudyInstanceUID |  |
| CurrentReqEvidence_SeriesInstanceUID |  |
| CurrentReqEvidence_SOPClassUID |  |
| CurrentReqEvidence_SOPInstanceUID |  |
| DicomFileSize | |


## OTHER

### ImageTable

| Field | Description |
| ------------- | ------------- |
| PatientID |  |
| StudyInstanceUID |  |
| StudyDate |  |
| StudyTime |  |
| ModalitiesInStudy |  |
| StudyDescription |  |
| PatientBirthDate |  |
| AccessionNumber |  |
| PatientSex |  |
| PatientAge |  |
| NumberOfStudyRelatedInstances |  |
| SeriesInstanceUID |  |
| Modality |  |
| SourceApplicationEntityTitle |  |
| InstitutionName |  |
| ProcedureCodeSequence |  |
| ProtocolName |  |
| PerformedProcedureStepID |  |
| PerformedProcedureStepDescription |  |
| SeriesDescription |  |
| BodyPartExamined |  |
| DeviceSerialNumber |  |
| SeriesNumber |  |
| SequenceName |  |
| SOPInstanceUID |  |
| SeriesDate |  |
| SeriesTime |  |
| BurnedInAnnotation |  |
| RelativeFileArchiveURI |  |
| MessageGuid |  |
| SliceLocation |  |
| SliceThickness |  |
| SpacingBetweenSlices |  |
| ImageType |  |
| ManufacturerModelName |  |
| Manufacturer |  |
| PhotometricInterpretation |  |
| DicomFileSize | |

