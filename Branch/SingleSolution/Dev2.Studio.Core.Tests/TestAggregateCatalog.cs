﻿using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Dev2.Network.Execution;
using Dev2.Studio.Core.Interfaces;

namespace Dev2.Core.Tests
{
    public class FullTestAggregateCatalog : AggregateCatalog
    {
        public FullTestAggregateCatalog()
        {
            this.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(Bootstrapper))));
            this.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(IEnvironmentModel))));
            this.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(INetworkExecutionChannel))));
        }
    }

    public class FullStudioAggregateCatalog : AggregateCatalog
    {
        public FullStudioAggregateCatalog()
        {
            this.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(Bootstrapper))));
            this.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(IEnvironmentModel))));
        }
    }

    public class StudioTestAggregateCatalog : AggregateCatalog
    {
        public StudioTestAggregateCatalog()
        {
            this.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(Bootstrapper))));
        }
    }

    public class StudioCoreTestAggregateCatalog : AggregateCatalog
    {
        public StudioCoreTestAggregateCatalog()
        {
            this.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(IEnvironmentModel))));
        }
    }

    public class Dev2NetworkTestAggregateCatalog : AggregateCatalog
    {
        public Dev2NetworkTestAggregateCatalog()
        {
        }
    }

    public class UnlimitedFrameworkTestAggregateCatalog : AggregateCatalog
    {
        public UnlimitedFrameworkTestAggregateCatalog()
        {
            this.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(INetworkExecutionChannel))));
        }
    }
}