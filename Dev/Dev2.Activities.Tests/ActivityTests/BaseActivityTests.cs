using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ActivityUnitTests;
using Dev2.Activities;
using Dev2.Common.Interfaces.Diagnostics.Debug;
using Dev2.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unlimited.Applications.BusinessDesignStudio.Activities;
using Unlimited.Applications.BusinessDesignStudio.Activities.Utilities;

namespace Dev2.Tests.Activities.ActivityTests
{
    /// <summary>
    /// Summary description for DataSplitActivityTest
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class BaseActivityTests : BaseActivityUnitTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void CommentGetDebugInputOutputWithText()
        {
            var act = new DsfCommentActivity { Text = "SomeText" };

            List<DebugItem> inRes;
            List<DebugItem> outRes;

            const string dataList = "<ADL><recset1><field1/><field2/><field3/></recset1><recset2><id/><value/></recset2><OutVar1/></ADL>";
            const string dataListWithData = "<ADL>" +
                                            "<recset1>" +
                                            "<field1>1</field1><field2>a</field2><field3>Test1</field3>" +
                                            "</recset1>" +
                                            "<recset1>" +
                                            "<field1>2</field1><field2>b</field2><field3>Test2</field3>" +
                                            "</recset1>" +
                                            "<recset1>" +
                                            "<field1>3</field1><field2>a</field2><field3>Test3</field3>" +
                                            "</recset1>" +
                                            "<recset1>" +
                                            "<field1>4</field1><field2>a</field2><field3>Test4</field3>" +
                                            "</recset1>" +
                                            "<recset1>" +
                                            "<field1>5</field1><field2>c</field2><field3>Test5</field3>" +
                                            "</recset1>" +
                                            "<OutVar1/></ADL>";

            var result = CheckActivityDebugInputOutput(act, dataList,
                dataListWithData, out inRes, out outRes);

            // remove test datalist ;)
            DataListRemoval(result.DataListID);

            Assert.AreEqual(0, inRes.Count);
            Assert.AreEqual(1, outRes.Count);
            IList<IDebugItemResult> debugOutput = outRes[0].FetchResultsList();
            Assert.AreEqual(1, debugOutput.Count);
            Assert.AreEqual("SomeText", debugOutput[0].Value);
            Assert.AreEqual(DebugItemResultType.Value, debugOutput[0].Type);
        }

        [TestMethod]
        public void DsfBaseActivityWhenHasVariableShouldEvaluate()
        {
            //---------------Set up test pack-------------------
            var act = new MySimpleActivity();
            act.Input1 = "[[OutVar1]]";
            act.Input2 = "[[OutVar2]]";
            act.Result = "[[ResultVar]]";
            List<DebugItem> inRes;
            List<DebugItem> outRes;

            const string dataList = "<ADL><OutVar1/><OutVar2/><ResultVar/></ADL>";
            const string dataListWithData = "<ADL>" +
                                            "<OutVar1>TestVal</OutVar1><OutVar2>TestVal2</OutVar2></ADL>";

            

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = CheckActivityDebugInputOutput(act, dataList,
                dataListWithData, out inRes, out outRes);

            // remove test datalist ;)
            DataListRemoval(result.DataListID);
            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("DsfCommentActivity_UpdateForEachInputs")]
        public void DsfCommentActivity_UpdateForEachInputs_DoesNothing()
        {
            //------------Setup for test--------------------------
            var act = new DsfCommentActivity { Text = "SomeText" };
            var tuple1 = new Tuple<string, string>("Test1", "Test");
            //------------Execute Test---------------------------
            act.UpdateForEachInputs(new List<Tuple<string, string>> { tuple1 }, null);
            //------------Assert Results-------------------------
            Assert.AreEqual("SomeText", act.Text);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("DsfCommentActivity_UpdateForEachOutputs")]
        public void DsfCommentActivity_UpdateForEachOutputs_NullDoesNothing_DoesNothing()
        {
            //------------Setup for test--------------------------
            var act = new DsfCommentActivity { Text = "SomeText" };
            //------------Execute Test---------------------------
            act.UpdateForEachOutputs(null, null);
            //------------Assert Results-------------------------
            Assert.AreEqual("SomeText", act.Text);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("DsfCommentActivity_UpdateForEachOutputs")]
        public void DsfCommentActivity_UpdateForEachOutputsMoreThanTwoItems_DoesNothing()
        {
            //------------Setup for test--------------------------
            var act = new DsfCommentActivity { Text = "SomeText" };
            var tuple1 = new Tuple<string, string>("Test1", "Test");
            var tuple2 = new Tuple<string, string>("Test2", "Test");
            //------------Execute Test---------------------------
            act.UpdateForEachOutputs(new List<Tuple<string, string>> { tuple1, tuple2 }, null);
            //------------Assert Results-------------------------
            Assert.AreEqual("SomeText", act.Text);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("DsfCommentActivity_UpdateForEachOutputs")]
        public void DsfCommentActivity_UpdateForEachOutputs_UpdatesTextValue()
        {
            //------------Setup for test--------------------------
            var act = new DsfCommentActivity { Text = "SomeText" };
            var tuple1 = new Tuple<string, string>("Test1", "Test");
            //------------Execute Test---------------------------
            act.UpdateForEachOutputs(new List<Tuple<string, string>> { tuple1 }, null);
            //------------Assert Results-------------------------
            Assert.AreEqual("Test", act.Text);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("DsfCommentActivity_GetForEachInputs")]
        public void DsfCommentActivity_GetForEachInputs_WhenHasExpression_ReturnsInputList()
        {
            //------------Setup for test--------------------------
            var act = new DsfCommentActivity { Text = "SomeText" };
            //------------Execute Test---------------------------
            var dsfForEachItems = act.GetForEachInputs();
            //------------Assert Results-------------------------
            Assert.AreEqual(0, dsfForEachItems.Count);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("DsfCommentActivity_GetForEachOutputs")]
        public void DsfCommentActivity_GetForEachOutputs_WhenHasResult_ReturnsInputList()
        {
            //------------Setup for test--------------------------
            var act = new DsfCommentActivity { Text = "SomeText" };
            //------------Execute Test---------------------------
            var dsfForEachItems = act.GetForEachOutputs();
            //------------Assert Results-------------------------
            Assert.AreEqual(1, dsfForEachItems.Count);
            Assert.AreEqual("SomeText", dsfForEachItems[0].Name);
            Assert.AreEqual("SomeText", dsfForEachItems[0].Value);
        }
    }

    internal sealed class MySimpleActivity : DsfBaseActivity
    {
        public MySimpleActivity()
        {
            DisplayName = "MySimpleActivty";
        }

        public override string DisplayName { get; set; }

        [Inputs("My Input 1")]
        public string Input1 { get; set; }

        [Inputs("My Input 2")]
        public string Input2 { get; set; }

        protected override string PerformExecution(Dictionary<string,string> evaluatedValues)
        {
            var result = evaluatedValues["Input1"] + " - " + evaluatedValues["Input2"];
            return result;
        }
    }
}