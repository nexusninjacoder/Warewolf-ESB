﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.34209
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Dev2.Studio.UI.Specs.SanityTestSuite
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute()]
    public partial class SpecFloUI_FileAndFolder_Delete_AutoIDeature1Feature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "EXapmle Workflows.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "SpecFloUI_File and Folder - Delete_AutoIDeature1", "In order to avoid silly mistakes\r\nAs a math idiot\r\nI want to be told the sum of t" +
                    "wo numbers", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((TechTalk.SpecFlow.FeatureContext.Current != null) 
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "SpecFloUI_File and Folder - Delete_AutoIDeature1")))
            {
                Dev2.Studio.UI.Specs.SanityTestSuite.SpecFloUI_FileAndFolder_Delete_AutoIDeature1Feature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Testing Example Workflows")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SpecFloUI_File and Folder - Delete_AutoIDeature1")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("mytag")]
        public virtual void TestingExampleWorkflows()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Testing Example Workflows", new string[] {
                        "mytag"});
#line 7
this.ScenarioSetup(scenarioInfo);
#line 8
 testRunner.Given("I have Warewolf running", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 9
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
 testRunner.And("I click \"EXPLORER,UI_localhost_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
 testRunner.And("I click \"EXPLORERFILTERCLEARBUTTON\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 13
 testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Assign_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 14
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Assign_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 15
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Assign(FlowchartDesigner)\" is visible within \"10\" sec" +
                    "onds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 16
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 18
    testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Comment_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 19
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Comment_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 20
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Comment(FlowchartDesigner)\" is visible within \"10\" se" +
                    "conds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 21
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Date and Time Difference" +
                    "_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 24
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Date and Time Dif" +
                    "ference_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 25
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Date and Time Difference(FlowchartDesigner)\" is visib" +
                    "le within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 26
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 28
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Date and Time_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 29
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Date and Time_Aut" +
                    "oID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 30
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Date and Time(FlowchartDesigner)\" is visible within \"" +
                    "10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 31
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 33
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Calculate_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 34
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Calculate_AutoID\"" +
                    "", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 35
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Calculate(FlowchartDesigner)\" is visible within \"10\" " +
                    "seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 36
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 38
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Find Index_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 39
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Find Index_AutoID" +
                    "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 40
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Find Index(FlowchartDesigner)\" is visible within \"10\"" +
                    " seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 41
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 43
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Format Number_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 44
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Format Number_Aut" +
                    "oID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 45
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Format Number(FlowchartDesigner)\" is visible within \"" +
                    "10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 46
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 48
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Random_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 49
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Random_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 50
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Random(FlowchartDesigner)\" is visible within \"10\" sec" +
                    "onds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 51
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 53
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Replace_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 54
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Replace_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 55
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Replace(FlowchartDesigner)\" is visible within \"10\" se" +
                    "conds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 56
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 58
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - System Information_AutoI" +
                    "D\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 59
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Utility - System Information_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 60
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - System Information(FlowchartDesigner)\" is visible wit" +
                    "hin \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 61
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 63
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Web Request_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 64
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - Web Request_AutoI" +
                    "D\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 65
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - Web Request(FlowchartDesigner)\" is visible within \"10" +
                    "\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 66
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 69
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - XPath_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 70
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Utility - XPath_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 71
 testRunner.Then("\"WORKFLOWDESIGNER,Utility - XPath(FlowchartDesigner)\" is visible within \"10\" seco" +
                    "nds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 72
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 74
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Control Flow - Decision_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 75
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Control Flow - Decision_Aut" +
                    "oID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 76
 testRunner.Then("\"WORKFLOWDESIGNER,Control Flow - Decision(FlowchartDesigner)\" is visible within \"" +
                    "10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 77
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 79
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Control Flow - Sequence_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 80
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Control Flow - Sequence_Aut" +
                    "oID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 81
 testRunner.Then("\"WORKFLOWDESIGNER,Control Flow - Sequence(FlowchartDesigner)\" is visible within \"" +
                    "10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 82
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 84
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Control Flow - Switch_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 85
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Control Flow - Switch_AutoI" +
                    "D\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 86
 testRunner.Then("\"WORKFLOWDESIGNER,UI_Control Flow - Switch_AutoID(FlowchartDesigner)\" is visible " +
                    "within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 87
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 89
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Rename_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 90
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Rename_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 91
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Rename(FlowchartDesigner)\" is visible within " +
                    "\"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 92
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 95
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Unzip_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 96
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Unzip_Aut" +
                    "oID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 97
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Unzip(FlowchartDesigner)\" is visible within \"" +
                    "10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 98
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 100
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Data - Case Conversion_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 101
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Data - Case Conversion_Auto" +
                    "ID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 102
 testRunner.Then("\"WORKFLOWDESIGNER,Data - Case Conversion(FlowchartDesigner)\" is visible within \"1" +
                    "0\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 103
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 106
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Write File_AutoI" +
                    "D\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 107
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Write Fil" +
                    "e_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 108
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Write File(FlowchartDesigner)\" is visible wit" +
                    "hin \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 109
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 111
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 112
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 113
 testRunner.Then("\"WORKFLOWDESIGNER,UI_File and Folder - Delete_AutoID1(FlowchartDesigner)\" is visi" +
                    "ble within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 114
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 117
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Data - Data Merge_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 118
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Data - Data Merge_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 119
 testRunner.Then("\"WORKFLOWDESIGNER,Data - Data Merge(FlowchartDesigner)\" is visible within \"10\" se" +
                    "conds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 120
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 123
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Zip_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 124
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Zip_AutoI" +
                    "D\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 125
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Zip(FlowchartDesigner)\" is visible within \"10" +
                    "\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 126
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 128
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 129
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 130
 testRunner.Then("\"WORKFLOWDESIGNER,UI_File and Folder - Delete_AutoID1(FlowchartDesigner)\" is visi" +
                    "ble within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 131
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 133
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Data - Data Split_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 134
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Data - Data Split_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 135
 testRunner.Then("\"WORKFLOWDESIGNER,Data - Data Split(FlowchartDesigner)\" is visible within \"10\" se" +
                    "conds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 136
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 138
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Create_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 139
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoIDUI_File and Folder - Create_Aut" +
                    "oID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 140
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Create(FlowchartDesigner)\" is visible within " +
                    "\"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 141
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 144
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Copy_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 145
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Copy_Auto" +
                    "ID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 146
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Copy(FlowchartDesigner)\" is visible within \"1" +
                    "0\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 147
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 149
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 150
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 151
 testRunner.Then("\"WORKFLOWDESIGNER,UI_File and Folder - Delete_AutoID1(FlowchartDesigner)\" is visi" +
                    "ble within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 152
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 154
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Loop Constructs - For Each_AutoID\"" +
                    "", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 155
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Loop Constructs - For Each_" +
                    "AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 156
 testRunner.Then("\"WORKFLOWDESIGNER,Loop Constructs - For Each(FlowchartDesigner)\" is visible withi" +
                    "n \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 157
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 159
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 160
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 161
 testRunner.Then("\"WORKFLOWDESIGNER,UI_File and Folder - Delete_AutoID1(FlowchartDesigner)\" is visi" +
                    "ble within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 162
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 164
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Move_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 165
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Move_Auto" +
                    "ID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 166
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Move(FlowchartDesigner)\" is visible within \"1" +
                    "0\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 167
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 169
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 170
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 171
 testRunner.Then("\"WORKFLOWDESIGNER,UI_File and Folder - Delete_AutoID1(FlowchartDesigner)\" is visi" +
                    "ble within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 172
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 174
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Read File_AutoID" +
                    "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 175
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Read File" +
                    "_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 176
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Read File(FlowchartDesigner)\" is visible with" +
                    "in \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 177
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 179
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Count Records_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 180
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Count Records_A" +
                    "utoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 181
 testRunner.Then("\"WORKFLOWDESIGNER,Recordset - Count Records(FlowchartDesigner)\" is visible within" +
                    " \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 182
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 184
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 185
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 186
 testRunner.Then("\"WORKFLOWDESIGNER,UI_File and Folder - Delete_AutoID1(FlowchartDesigner)\" is visi" +
                    "ble within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 187
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 189
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Read Folder_Auto" +
                    "ID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 190
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Read Fold" +
                    "er_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 191
 testRunner.Then("\"WORKFLOWDESIGNER,File and Folder - Read Folder(FlowchartDesigner)\" is visible wi" +
                    "thin \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 192
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 194
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 195
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 196
 testRunner.Then("\"WORKFLOWDESIGNER,UI_File and Folder - Delete_AutoID1(FlowchartDesigner)\" is visi" +
                    "ble within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 197
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 199
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Delete Records_AutoID\"" +
                    "", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 200
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Delete Records_" +
                    "AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 201
 testRunner.Then("\"WORKFLOWDESIGNER,Recordset - Delete Records(FlowchartDesigner)\" is visible withi" +
                    "n \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 202
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 204
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Find Records_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 205
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Find Records_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 206
 testRunner.Then("\"WORKFLOWDESIGNER,Recordset - Find Records(FlowchartDesigner)\" is visible within " +
                    "\"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 207
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 209
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Records Length_AutoID\"" +
                    "", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 210
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Records Length_" +
                    "AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 211
 testRunner.Then("\"WORKFLOWDESIGNER,Recordset - Records Length(FlowchartDesigner)\" is visible withi" +
                    "n \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 212
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 214
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Sort Records_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 215
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Sort Records_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 216
 testRunner.Then("\"WORKFLOWDESIGNER,Recordset - Sort Records(FlowchartDesigner)\" is visible within " +
                    "\"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 217
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 219
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - SQL Bulk Insert_AutoID" +
                    "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 220
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - SQL Bulk Insert" +
                    "_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 221
 testRunner.Then("\"WORKFLOWDESIGNER,UI_Recordset - SQL Bulk Insert_AutoID(FlowchartDesigner)\" is vi" +
                    "sible within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 222
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 224
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Unique Records_AutoID\"" +
                    "", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 225
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Recordset - Unique Records_" +
                    "AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 226
 testRunner.Then("\"WORKFLOWDESIGNER,Recordset - Unique Records(FlowchartDesigner)\" is visible withi" +
                    "n \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 227
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 229
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 230
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_File and Folder - Delete_Au" +
                    "toID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 231
 testRunner.Then("\"WORKFLOWDESIGNER,UI_File and Folder - Delete_AutoID1(FlowchartDesigner)\" is visi" +
                    "ble within \"10\" seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 232
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 234
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Scripting - CMD Line_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 235
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Scripting - CMD Line_AutoID" +
                    "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 236
 testRunner.Then("\"WORKFLOWDESIGNER,Scripting - CMD Line(FlowchartDesigner)\" is visible within \"10\"" +
                    " seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 237
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 239
  testRunner.And("I click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Scripting - Script_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 240
 testRunner.And("I double click \"EXPLORERFOLDERS,UI_Examples_AutoID,UI_Scripting - Script_AutoID\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 241
 testRunner.Then("\"WORKFLOWDESIGNER,Scripting - Script(FlowchartDesigner)\" is visible within \"10\" s" +
                    "econds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 242
 testRunner.And("all tabs are closed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
