﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dev2.Interfaces
{
    public interface ICaseConvertTO : IDev2TOFn
    {
        string StringToConvert { get; set; }
        string ConvertType { get; set; }
        IList<string> Expressions { get; set; }
        string ExpressionToConvert { get; set; }
        string Result { get; set; }        
        string WatermarkTextVariable { get; set; }
        
    }
}