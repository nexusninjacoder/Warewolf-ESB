﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by coded UI test builder.
//      Version: 11.0.0.0
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

using System.Text;
using System.Threading;
using System.Windows.Forms;
using Dev2.CodedUI.Tests;
using Dev2.CodedUI.Tests.UIMaps.DocManagerUIMapClasses;
using Dev2.CodedUI.Tests.UIMaps.ExplorerUIMapClasses;
using Dev2.CodedUI.Tests.UIMaps.RibbonUIMapClasses;

namespace Dev2.Studio.UI.Tests.UIMaps.WebServiceWizardUIMapClasses
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;

    public partial class WebServiceWizardUIMap : UIMapBase
    {
        public void     InitializeFullTestServiceAndSource(string serviceName, string sourceName)
        {
            //Open wizard
            RibbonUIMap.ClickRibbonMenuItem("New Web Service");

            //Wait for wizard
            WizardsUIMap.WaitForWizard();

            //Click new web source
            WebServiceWizardUIMap.ClickNewWebSource();

            //Web Source Details
            SendKeys.SendWait("http://www.webservicex.net/globalweather.asmx{TAB}{TAB}{TAB}");
            Playback.Wait(100);
            SendKeys.SendWait("{ENTER}");
            Playback.Wait(10000 );
            WebSourceWizardUIMap.ClickSave();
            SendKeys.SendWait("{TAB}{TAB}{TAB}" + sourceName + "{TAB}{ENTER}");
            Playback.Wait(1000);

            //Web Service Details
            SendKeys.SendWait("{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}{TAB}");
            Playback.Wait(500);
            SendKeys.SendWait("{ENTER}");
            Playback.Wait(30000);//wait for test
            SendKeys.SendWait("{TAB}{ENTER}");
            Playback.Wait(1000);
            SendKeys.SendWait("{TAB}{TAB}{TAB}" + serviceName + "{TAB}{ENTER}");
            
        }

        public static void Cancel()
        {
            for (var i = 0; i < 4; i++)
            {
                Keyboard.SendKeys("{TAB}");
            }
            Keyboard.SendKeys("{ENTER}");
        }
    }

    [GeneratedCode("Coded UITest Builder", "11.0.60315.1")]
    public class UIStartPageCustom : WpfCustom
    {
        
        public UIStartPageCustom(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Uia.ContentPane";
            this.SearchProperties["AutomationId"] = "splurt";
            this.WindowTitles.Add(TestBase.GetStudioWindowName());
            #endregion
        }
        
        #region Properties
        public WpfImage UIItemImage
        {
            get
            {
                if ((this.mUIItemImage == null))
                {
                    this.mUIItemImage = new WpfImage(this);
                    #region Search Criteria
                    this.mUIItemImage.WindowTitles.Add(TestBase.GetStudioWindowName());
                    #endregion
                }
                return this.mUIItemImage;
            }
        }
        #endregion
        
        #region Fields
        private WpfImage mUIItemImage;
        #endregion
    }
}