﻿using Dev2.Studio.Core.Interfaces;
using System;
using System.Activities.Presentation.Model;

namespace Dev2.Studio.Core 
{
    public class WebActivity : IWebActivity
    {
        public string WebsiteServiceName {
            get { return GetPropertyValue(WebActivityObject, "WebsiteServiceName"); }
            set { SetPropertyValue(WebActivityObject, "WebsiteServiceName", value); }
        }

        public string MetaTags {
            get { return GetPropertyValue(WebActivityObject, "MetaTags"); }
            set { SetPropertyValue(WebActivityObject, "MetaTags", value); }
        }

        public string FormEncodingType {
            get { return GetPropertyValue(WebActivityObject, "FormEncodingType"); }
            set { SetPropertyValue(WebActivityObject, "FormEncodingType", value); }
        }

        public string XMLConfiguration {
            get { return GetPropertyValue(WebActivityObject, "XMLConfiguration"); }
            set { SetPropertyValue(WebActivityObject, "XMLConfiguration", value); }
        }

        public string Html {
            get { return GetPropertyValue(WebActivityObject, "Html"); }
            set { SetPropertyValue(WebActivityObject, "Html", value); }
        }

        public string ServiceName
        {
            get { return GetPropertyValue(WebActivityObject, "ServiceName"); }
            set { SetPropertyValue(WebActivityObject, "ServiceName", value); }
        }

        public string LiveInputMapping { get; set; }
        public string LiveOutputMapping { get; set; }

        public string SavedInputMapping {
            get { return GetPropertyValue(WebActivityObject, "InputMapping"); }
            set { SetPropertyValue(WebActivityObject, "InputMapping", value); }
        }

        public string SavedOutputMapping {
            get { return GetPropertyValue(WebActivityObject, "OutputMapping"); }
            set { SetPropertyValue(WebActivityObject, "OutputMapping", value); }
        }

        public object WebActivityObject { get; set; }

        public Type UnderlyingWebActivityObjectType {
            get {
                if ((WebActivityObject != null) && WebActivityObject is ModelItem) {
                    return (WebActivityObject as ModelItem).ItemType;
                }

                return null;
            }
        }


        public IContextualResourceModel ResourceModel { get; set; }

        private bool ContainsProperty(object modelItemObject, string propertyName) {
            var modelItem = modelItemObject as ModelItem;
            if (modelItem != null) {
                return modelItem.Properties[propertyName] != null;
            }

            return false;
        }

        private string GetPropertyValue(object modelItemObject, string propertyName) {
            var modelItem = modelItemObject as ModelItem;
            if (modelItem != null && modelItem.Properties[propertyName] != null) {
                return modelItem.Properties[propertyName].ComputedValue == null
                           ? string.Empty
                           : modelItem.Properties[propertyName].ComputedValue.ToString();
            }
            return string.Empty;
        }

        private void SetPropertyValue(object modelItemObject, string propertyName, object value) {
            var modelItem = modelItemObject as ModelItem;
            if (modelItem != null && modelItem.Properties[propertyName] != null) {
                modelItem.Properties[propertyName].SetValue(value);
            }
        }
    }
}