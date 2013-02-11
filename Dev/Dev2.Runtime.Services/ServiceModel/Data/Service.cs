﻿using System.Xml.Linq;

namespace Dev2.Runtime.ServiceModel.Data
{
    public class Service : Resource
    {
        #region CTOR

        public Service()
        {
        }

        public Service(XElement xml)
            : base(xml)
        {
        }

        #endregion

        #region ToXml

        public override XElement ToXml()
        {
            var result = base.ToXml();
            result.Add(new XAttribute("MethodName", MethodName ?? string.Empty));

            return result;
        }

        #endregion

        public string MethodName { get; set; }
        public MethodParameterList MethodParameters { get; set; }
        public MethodOutputList MethodOutputs { get; set; }
    }
}
