﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.3.2.0
//      SpecFlow Generator Version:2.3.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Sfa.Das.ApprenticeshipInfoService.AcceptanceTests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.3.2.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Frameworks")]
    public partial class FrameworksFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Frameworks.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Frameworks", "    Get ALL the Frameworks", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
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
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Verify correct status code is returned")]
        [NUnit.Framework.TestCaseAttribute("frameworks", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/403-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/codes", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/codes/403", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/403-2-6", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/405-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/409-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/473-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/477-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/499-3-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/506-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/506-2-2", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/532-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/475-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/427-3-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/506-3-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/473-3-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/506-3-2", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/513-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/602-21-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/479-2-1", "OK", null)]
        [NUnit.Framework.TestCaseAttribute("frameworks/473-2-1", "OK", null)]
        public virtual void VerifyCorrectStatusCodeIsReturned(string uri, string code, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Verify correct status code is returned", exampleTags);
#line 5
this.ScenarioSetup(scenarioInfo);
#line 6
    testRunner.Given(string.Format("I send request to {0}", uri), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 7
    testRunner.Then(string.Format("I get response code {0} is returned", code), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Verify correct amount of results returned")]
        public virtual void VerifyCorrectAmountOfResultsReturned()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Verify correct amount of results returned", ((string[])(null)));
#line 34
this.ScenarioSetup(scenarioInfo);
#line 35
 testRunner.Given("I send request to frameworks", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 36
 testRunner.Then("I expect the amount of refult will be at least 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
