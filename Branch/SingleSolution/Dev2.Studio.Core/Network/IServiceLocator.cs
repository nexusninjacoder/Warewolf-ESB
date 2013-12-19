﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dev2.Studio.Core.Network
{
    public interface IServiceLocator
    {
        void RegisterEnpoint<T>(string key, Func<T, Uri> enpointGenerationStrategy);
        void RegisterEnpoint(string key, Uri endpoint);

        Uri GetEndpoint(string key);
        Uri GetEndpoint<T>(string key, T param);
    }
}