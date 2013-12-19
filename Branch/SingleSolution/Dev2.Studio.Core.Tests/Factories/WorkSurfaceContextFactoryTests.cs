﻿using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Dev2.Studio.Core.Models;
using Dev2.Studio.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev2.Core.Tests.Factories
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WorkSurfaceContextFactoryTests
    {

        [TestMethod]
        [Owner("Ashley Lewis")]
        [TestCategory("WorkSurfaceContextFactory_CreateDeployViewModel")]
        public void WorkSurfaceContextFactory_CreateDeployViewModel_ResourceModel_DeployViewModelInitialized()
        {
            var thread = new Thread(() =>
            {
                CompositionInitializer.DefaultInitialize();
                var mock = NavigationViewModelTest.GetMockEnvironment();
                var resourceModel = new ResourceModel(mock.Object);
                resourceModel.ResourceName = "Expected Resource";

                //------------Execute Test---------------------------
                var actual = WorkSurfaceContextFactory.CreateDeployViewModel(resourceModel);

                // Assert DeployViewModelInitialized
                Assert.IsNotNull(actual, "Cannot create DeployWorkSurface with resource model");
            });
        }
    }
}