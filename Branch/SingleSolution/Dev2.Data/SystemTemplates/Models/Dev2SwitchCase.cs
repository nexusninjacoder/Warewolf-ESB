﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dev2.Data.SystemTemplates.Models
{
    /// <summary>
    /// A model item for the Switch Case on the workflow designer
    /// </summary>
    public class Dev2SwitchCase : IDev2DataModel
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [JsonIgnore]
        public string Version
        {
            get { return "1.0.0"; }
        }

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        /// <value>
        /// The name of the model.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public Dev2ModelType ModelName
        {
            get { return Dev2ModelType.Dev2SwitchCase; }
        }

        /// <summary>
        /// Gets or sets the case value.
        /// </summary>
        /// <value>
        /// The case value.
        /// </value>
        public string CaseValue { get; set; }

        public string ToWebModel()
        {
            return JsonConvert.SerializeObject(this);
        }
        
        public string GenerateUserFriendlyModel(Guid dlid, Dev2DecisionMode mode)
        {
            return "For " + CaseValue;
        }
    }
}