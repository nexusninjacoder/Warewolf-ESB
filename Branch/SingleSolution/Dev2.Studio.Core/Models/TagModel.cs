﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dev2.Studio.Core.Models {
    public class TagModel {
        public string Tag { get; set; }
        public object Resource { get; set; }
        public bool IsSelected { get; set; }
    }
}