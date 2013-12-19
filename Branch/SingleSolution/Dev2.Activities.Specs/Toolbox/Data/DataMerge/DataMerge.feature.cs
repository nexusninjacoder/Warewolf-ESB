﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.18052
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Dev2.Activities.Specs.Toolbox.Data.DataMerge
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class DataMergeFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "DataMerge.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "DataMerge", "In order to merge data\r\nAs Warewolf user\r\nI want a tool that joins two or more pi" +
                    "eces of data together", ProgrammingLanguage.CSharp, ((string[])(null)));
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
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "DataMerge")))
            {
                Dev2.Activities.Specs.Toolbox.Data.DataMerge.DataMergeFeature.FeatureSetup(null);
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
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a scalar to a scalar using merge type none")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeAScalarToAScalarUsingMergeTypeNone()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a scalar to a scalar using merge type none", ((string[])(null)));
#line 6
this.ScenarioSetup(scenarioInfo);
#line 7
 testRunner.Given("a merge variable \"[[a]]\" equal to \"Warewolf \"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
 testRunner.And("a merge variable \"[[b]]\" equal to \"Rocks\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
 testRunner.And("an Input \"[[a]]\" and merge type \"None\" and string at as \"\" and Padding \"\" and Ali" +
                    "gnment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
 testRunner.And("an Input \"[[b]]\" and merge type \"None\" and string at as \"\" and Padding \"\" and Ali" +
                    "gnment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 12
 testRunner.Then("the merged result is \"Warewolf Rocks\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 13
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a recordset table and free text using None")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeARecordsetTableAndFreeTextUsingNone()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a recordset table and free text using None", ((string[])(null)));
#line 15
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "rs",
                        "val"});
            table1.AddRow(new string[] {
                        "rs().row",
                        "1"});
            table1.AddRow(new string[] {
                        "rs().row",
                        "2"});
            table1.AddRow(new string[] {
                        "rs().row",
                        "3"});
#line 16
 testRunner.Given("a merge recordset", ((string)(null)), table1, "Given ");
#line 21
 testRunner.And("an Input \"[[rs(*).row]]0\" and merge type \"None\" and string at as \"\" and Padding \"" +
                    "\" and Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
 testRunner.And("an Input \"0\" and merge type \"None\" and string at as \"\" and Padding \"\" and Alignme" +
                    "nt \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 24
 testRunner.Then("the merged result is \"100200300\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 25
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a recordset table and free text using Chars")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeARecordsetTableAndFreeTextUsingChars()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a recordset table and free text using Chars", ((string[])(null)));
#line 27
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "rs",
                        "val"});
            table2.AddRow(new string[] {
                        "rs().row",
                        "1"});
            table2.AddRow(new string[] {
                        "rs().row",
                        "2"});
            table2.AddRow(new string[] {
                        "rs().row",
                        "3"});
#line 28
 testRunner.Given("a merge recordset", ((string)(null)), table2, "Given ");
#line 33
 testRunner.And("an Input \"[[rs(*).row]]\" and merge type \"Chars\" and string at as \"0\" and Padding " +
                    "\"\" and Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 34
 testRunner.And("an Input \"0\" and merge type \"Chars\" and string at as \"0\" and Padding \"\" and Align" +
                    "ment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 35
 testRunner.And("an Input \"0\" and merge type \"None\" and string at as \"\" and Padding \"\" and Alignme" +
                    "nt \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 36
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 37
 testRunner.Then("the merged result is \"100002000030000\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 38
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a recordset table and free text using New Line")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeARecordsetTableAndFreeTextUsingNewLine()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a recordset table and free text using New Line", ((string[])(null)));
#line 40
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "rs",
                        "val"});
            table3.AddRow(new string[] {
                        "rs().row",
                        "1"});
            table3.AddRow(new string[] {
                        "rs().row",
                        "2"});
            table3.AddRow(new string[] {
                        "rs().row",
                        "3"});
#line 41
 testRunner.Given("a merge recordset", ((string)(null)), table3, "Given ");
#line 46
 testRunner.And("an Input \"[[rs(*).row]]\" and merge type \"New Line\" and string at as \"\" and Paddin" +
                    "g \"\" and Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 47
 testRunner.And("an Input \"0\" and merge type \"New Line\" and string at as \"\" and Padding \"\" and Ali" +
                    "gnment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 48
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 49
 testRunner.Then("the merged result is the same as file \"NewLineExample.txt\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 50
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a recordset table and free text using Tab")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeARecordsetTableAndFreeTextUsingTab()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a recordset table and free text using Tab", ((string[])(null)));
#line 52
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "rs",
                        "val"});
            table4.AddRow(new string[] {
                        "rs().row",
                        "1"});
            table4.AddRow(new string[] {
                        "rs().row",
                        "2"});
            table4.AddRow(new string[] {
                        "rs().row",
                        "3"});
#line 53
 testRunner.Given("a merge recordset", ((string)(null)), table4, "Given ");
#line 58
 testRunner.And("an Input \"[[rs(*).row]]tab->\" and merge type \"Tab\" and string at as \"\" and Paddin" +
                    "g \"\" and Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 59
 testRunner.And("an Input \"<-\" and merge type \"None\" and string at as \"\" and Padding \"\" and Alignm" +
                    "ent \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 60
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 61
 testRunner.Then("the merged result is \"1tab->\t<-2tab->\t<-3tab->\t<-\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 62
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a variable using index that is a char")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeAVariableUsingIndexThatIsAChar()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a variable using index that is a char", ((string[])(null)));
#line 64
this.ScenarioSetup(scenarioInfo);
#line 65
 testRunner.Given("a merge variable \"[[a]]\" equal to \"aA \"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 66
 testRunner.And("an Input \"[[a]]\" and merge type \"Index\" and string at as \"b\" and Padding \"\" and A" +
                    "lignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 67
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 68
 testRunner.Then("the merged result is \"\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 69
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a variable using index that is a variable and is blank")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeAVariableUsingIndexThatIsAVariableAndIsBlank()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a variable using index that is a variable and is blank", ((string[])(null)));
#line 71
this.ScenarioSetup(scenarioInfo);
#line 72
 testRunner.Given("a merge variable \"[[a]]\" equal to \"aA \"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 73
 testRunner.And("a merge variable \"[[b]]\" equal to \"\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 74
 testRunner.And("an Input \"[[a]]\" and merge type \"Index\" and string at as \"[[b]]\" and Padding \"\" a" +
                    "nd Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 75
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 76
 testRunner.Then("the merged result is \"\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 77
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge multiple variables on Chars with blank lines")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeMultipleVariablesOnCharsWithBlankLines()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge multiple variables on Chars with blank lines", ((string[])(null)));
#line 79
this.ScenarioSetup(scenarioInfo);
#line 80
 testRunner.Given("a merge variable \"[[a]]\" equal to \"Warewolf \"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 81
 testRunner.And("a merge variable \"[[b]]\" equal to \"Rocks\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 82
 testRunner.And("an Input \"[[a]]\" and merge type \"Chars\" and string at as \"|\" and Padding \" \" and " +
                    "Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 83
 testRunner.And("an Input \"\" and merge type \"None\" and string at as \"\" and Padding \" \" and Alignme" +
                    "nt \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 84
 testRunner.And("an Input \"[[b]]\" and merge type \"Chars\" and string at as \"|\" and Padding \" \" and " +
                    "Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 85
 testRunner.And("an Input \"\" and merge type \"Chars\" and string at as \"|\" and Padding \" \" and Align" +
                    "ment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 86
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 87
 testRunner.Then("the merged result is \"Warewolf |Rocks||\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 88
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a recordset that has xml data using Tabs")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeARecordsetThatHasXmlDataUsingTabs()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a recordset that has xml data using Tabs", ((string[])(null)));
#line 90
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "rs",
                        "val"});
            table5.AddRow(new string[] {
                        "rs().row",
                        "<x id=\"1\">One</x>"});
            table5.AddRow(new string[] {
                        "rs().row",
                        "<x id=\"2\">two</x>"});
            table5.AddRow(new string[] {
                        "rs().row",
                        "<x id=\"3\">three</x>"});
#line 91
 testRunner.Given("a merge recordset", ((string)(null)), table5, "Given ");
#line 96
 testRunner.And("an Input \"<record>\" and merge type \"Tab\" and string at as \"\" and Padding \"\" and A" +
                    "lignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 97
 testRunner.And("an Input \"[[rs(*).row]]\" and merge type \"Tab\" and string at as \"\" and Padding \"\" " +
                    "and Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 98
 testRunner.And("an Input \"</record>\" and merge type \"None\" and string at as \"\" and Padding \"\" and" +
                    " Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 99
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 100
 testRunner.Then("the merged result is \"<record>\t<x id=\"1\">One</x>\t</record><record>\t<x id=\"2\">two<" +
                    "/x>\t</record><record>\t<x id=\"3\">three</x>\t</record>\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 101
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a short string using big index and padding and alignment")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeAShortStringUsingBigIndexAndPaddingAndAlignment()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a short string using big index and padding and alignment", ((string[])(null)));
#line 103
this.ScenarioSetup(scenarioInfo);
#line 104
 testRunner.Given("a merge variable \"[[a]]\" equal to \"Warewolf\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 105
 testRunner.And("a merge variable \"[[b]]\" equal to \"123\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 106
 testRunner.And("an Input \"[[a]]\" and merge type \"Index\" and string at as \"10\" and Padding \" \" and" +
                    " Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 107
 testRunner.And("an Input \"[[b]]\" and merge type \"Index\" and string at as \"5\" and Padding \"0\" and " +
                    "Alignment \"Right\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 108
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 109
 testRunner.Then("the merged result is \"Warewolf  00123\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 110
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a long string using small index and padding and alignment")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeALongStringUsingSmallIndexAndPaddingAndAlignment()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a long string using small index and padding and alignment", ((string[])(null)));
#line 112
this.ScenarioSetup(scenarioInfo);
#line 113
 testRunner.Given("a merge variable \"[[a]]\" equal to \"Warewolf\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 114
 testRunner.And("a merge variable \"[[b]]\" equal to \"12345\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 115
 testRunner.And("an Input \"[[a]]\" and merge type \"Index\" and string at as \"3\" and Padding \" \" and " +
                    "Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 116
 testRunner.And("an Input \"[[b]]\" and merge type \"Index\" and string at as \"3\" and Padding \"0\" and " +
                    "Alignment \"Right\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 117
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 118
 testRunner.Then("the merged result is \"War123\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 119
 testRunner.And("the data merge execution has \"NO\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a long string using small index and padding and alignment at invalid index")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeALongStringUsingSmallIndexAndPaddingAndAlignmentAtInvalidIndex()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a long string using small index and padding and alignment at invalid index", ((string[])(null)));
#line 122
this.ScenarioSetup(scenarioInfo);
#line 123
 testRunner.Given("a merge variable \"[[a]]\" equal to \"Warewolf\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 124
 testRunner.And("a merge variable \"[[b]]\" equal to \"12345\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 125
 testRunner.And("an Input \"[[a]]\" and merge type \"Index\" and string at as \"-1\" and Padding \" \" and" +
                    " Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 126
 testRunner.And("an Input \"[[b]]\" and merge type \"Index\" and string at as \"-1\" and Padding \"0\" and" +
                    " Alignment \"Right\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 127
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 128
 testRunner.Then("the merged result is \"\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 129
 testRunner.And("the data merge execution has \"AN\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a negative recordset index Input")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeANegativeRecordsetIndexInput()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a negative recordset index Input", ((string[])(null)));
#line 131
this.ScenarioSetup(scenarioInfo);
#line 132
 testRunner.Given("an Input \"[[my(-1).a]]\" and merge type \"Index\" and string at as \"10\" and Padding " +
                    "\" \" and Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 133
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 134
 testRunner.Then("the data merge execution has \"AN\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a negative recordset index for String At")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeANegativeRecordsetIndexForStringAt()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a negative recordset index for String At", ((string[])(null)));
#line 136
this.ScenarioSetup(scenarioInfo);
#line 137
 testRunner.Given("an Input \"12\" and merge type \"Index\" and string at as \"[[my(-1).a]]\" and Padding " +
                    "\" \" and Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 138
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 139
 testRunner.Then("the data merge execution has \"AN\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Merge a negative recordset index for Padding")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "DataMerge")]
        public virtual void MergeANegativeRecordsetIndexForPadding()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Merge a negative recordset index for Padding", ((string[])(null)));
#line 141
this.ScenarioSetup(scenarioInfo);
#line 142
 testRunner.Given("an Input \"12\" and merge type \"Index\" and string at as \"10\" and Padding \"[[my(-1)." +
                    "a]]\" and Alignment \"Left\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 143
 testRunner.When("the data merge tool is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 144
 testRunner.Then("the data merge execution has \"AN\" error", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion