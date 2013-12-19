﻿using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Dev2.Activities;
using Dev2.Enums;
using Dev2.Factories;
using Dev2.Interfaces;
using Dev2.Runtime.ServiceModel.Data;
using Dev2.TO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unlimited.Applications.BusinessDesignStudio.Activities;

namespace Dev2.Tests.Activities.FindMissingStrategyTest
{
    /// <summary>
    /// Summary description for MixedActivityFindMissingStrategyTests
    /// </summary>
    [TestClass][ExcludeFromCodeCoverage]
    public class MixedActivityFindMissingStrategyTests
    {
        public MixedActivityFindMissingStrategyTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        #region DataSplit Activity Tests

        [TestMethod]
        public void GetActivityFieldsOffDataSplitActivityExpectedAllFindMissingFieldsToBeReturned()
        {
            DsfDataSplitActivity dataSplitActivity = new DsfDataSplitActivity();
            dataSplitActivity.ResultsCollection = new List<DataSplitDTO> { new DataSplitDTO("[[OutputVariable1]]", "Index", "[[At1]]", 1), new DataSplitDTO("[[OutputVariable2]]", "Index", "[[At2]]", 2) };
            dataSplitActivity.SourceString = "[[SourceString]]";
            Dev2FindMissingStrategyFactory fac = new Dev2FindMissingStrategyFactory();
            IFindMissingStrategy strategy = fac.CreateFindMissingStrategy(enFindMissingType.MixedActivity);
            List<string> actual = strategy.GetActivityFields(dataSplitActivity);
            List<string> expected = new List<string> { "[[OutputVariable1]]", "[[At1]]", "[[OutputVariable2]]", "[[At2]]", "[[SourceString]]" };
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region DataMerge Activity Tests

        [TestMethod]
        public void GetActivityFieldsOffDataMergeActivityExpectedAllFindMissingFieldsToBeReturned()
        {
            DsfDataMergeActivity dataMergeActivity = new DsfDataMergeActivity();
            dataMergeActivity.MergeCollection = new List<DataMergeDTO> { new DataMergeDTO("[[InputVariable1]]", "None", "[[At1]]", 1, "[[Padding1]]", "Left"), new DataMergeDTO("[[InputVariable2]]", "None", "[[At2]]", 2, "[[Padding2]]", "Left") };
            dataMergeActivity.Result = "[[Result]]";
            Dev2FindMissingStrategyFactory fac = new Dev2FindMissingStrategyFactory();
            IFindMissingStrategy strategy = fac.CreateFindMissingStrategy(enFindMissingType.MixedActivity);
            List<string> actual = strategy.GetActivityFields(dataMergeActivity);
            List<string> expected = new List<string> { "[[Padding1]]", "[[InputVariable1]]", "[[At1]]", "[[Padding2]]", "[[InputVariable2]]", "[[At2]]", "[[Result]]" };
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion


        [TestMethod]
        [Owner("Trevor Williams-Ros")]
        [TestCategory("MixedActivityFindMissingStrategy_GetActivityFields")]
        public void MixedActivityFindMissingStrategy_GetActivityFields_DsfSqlBulkInsertActivity_AllFindMissingFieldsToBeReturned()
        {
            //------------Setup for test--------------------------
            var activity = new DsfSqlBulkInsertActivity();
            activity.Result = "[[Result]]";
            activity.InputMappings = new List<DataColumnMapping>
            {
                new DataColumnMapping { InputColumn  = "[[rs().Field1]]", OutputColumn = new DbColumn()}, 
                new DataColumnMapping { InputColumn  = "[[rs().Field2]]", OutputColumn = new DbColumn()}, 
            };

            var fac = new Dev2FindMissingStrategyFactory();
            var strategy = fac.CreateFindMissingStrategy(enFindMissingType.MixedActivity);

            //------------Execute Test---------------------------
            var actual = strategy.GetActivityFields(activity);

            //------------Assert Results-------------------------
            var expected = new List<string> { "[[rs().Field1]]", "[[rs().Field2]]", "[[Result]]" };
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}