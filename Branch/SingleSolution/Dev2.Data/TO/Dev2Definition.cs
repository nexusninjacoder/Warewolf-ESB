﻿
namespace Dev2.DataList.Contract
{
    public class Dev2Definition : IDev2Definition
    {
        #region Ctor

        public Dev2Definition()
        {
        }

        public Dev2Definition(string name, string mapsTo, string value, bool isEvaluated, string defaultValue, bool isRequired, string rawValue) : this(name, mapsTo, value, string.Empty, isEvaluated, defaultValue, isRequired, rawValue) { }

        public Dev2Definition(string name, string mapsTo, string value, string recordSet, bool isEvaluated, string defaultValue, bool isRequired, string rawValue)
            : this(name, mapsTo, value, recordSet, isEvaluated, defaultValue, isRequired, rawValue, false)
        {
        }

        public Dev2Definition(string name, string mapsTo, string value, string recordSet, bool isEvaluated, string defaultValue, bool isRequired, string rawValue, bool emptyToNull)
        {
            Name = name;
            MapsTo = mapsTo;
            Value = value;
            RecordSetName = recordSet;
            IsEvaluated = isEvaluated;
            DefaultValue = defaultValue;
            IsRequired = isRequired;
            RawValue = rawValue;
            EmptyToNull = emptyToNull;
        }
        #endregion

        #region Properties

        public string Name { get; set; }

        public string MapsTo { get; set; }

        public string Value { get; set; }

        public bool IsRecordSet
        {
            get
            {
                return !((RecordSetName == null) || RecordSetName.Equals(string.Empty));
            }
        }

        public string RecordSetName { get; set; }

        public bool IsEvaluated { get; set; }

        public string DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public string RawValue { get; set; }

        public bool EmptyToNull { get; set; }
        #endregion
    }
}