﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unlimited.Applications.BusinessDesignStudio.Activities.Utilities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GeneralSettings : Attribute, IActivityPropertyAttribute
    {
        public GeneralSettings(string userVisibleName)
        {
            UserVisibleName = userVisibleName;
        }

        public string UserVisibleName { get; set; }
    }
}