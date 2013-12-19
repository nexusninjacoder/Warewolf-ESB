﻿using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using Dev2.Activities.Designers2.BaseConvert;
using Dev2.Common;
using Dev2.Converters;
using Dev2.Studio.Core.Activities.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;using System.Diagnostics.CodeAnalysis;
using Unlimited.Applications.BusinessDesignStudio.Activities;

// ReSharper disable InconsistentNaming
namespace Dev2.Activities.Designers.Tests.BaseConvert
{
    [TestClass][ExcludeFromCodeCoverage]
    public class BaseConvertTests
    {
        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("BaseConvertViewModel_Constructor")]
        public void BaseConvertViewModel_Constructor_PropertiesInitialized()
        {
            //------------Setup for test--------------------------
            var items = new List<BaseConvertTO>
            {
                new BaseConvertTO("xxxx","Text" ,"Binary","", 1),
                new BaseConvertTO("yyyy","Text" ,"Text","", 2)
            };

            //------------Execute Test---------------------------
            var viewModel = new BaseConvertDesignerViewModel(CreateModelItem(items));

            //------------Assert Results-------------------------
            Assert.IsNotNull(viewModel.ModelItem);
            Assert.IsNotNull(viewModel.ModelItemCollection);
            Assert.AreEqual("ConvertCollection", viewModel.CollectionName);

            var expectedOptions = Dev2EnumConverter.ConvertEnumsTypeToStringList<enDev2BaseConvertType>().ToList();
            CollectionAssert.AreEqual(expectedOptions, viewModel.ConvertTypes.ToList());

            Assert.AreEqual(2, viewModel.TitleBarToggles.Count);
        }


        static ModelItem CreateModelItem(IEnumerable<BaseConvertTO> items, string displayName = "Base Convert")
        {
            var modelItem = ModelItemUtils.CreateModelItem(new DsfBaseConvertActivity());
            modelItem.SetProperty("DisplayName", displayName);

            // ReSharper disable PossibleNullReferenceException
            var modelItemCollection = modelItem.Properties["ConvertCollection"].Collection;
            foreach(var dto in items)
            {
                modelItemCollection.Add(dto);
            }
            // ReSharper restore PossibleNullReferenceException

            return modelItem;
        }
    }
}