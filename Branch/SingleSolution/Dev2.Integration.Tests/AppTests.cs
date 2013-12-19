﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev2.Integration.Tests
{
    [TestClass]
    public class AppTests
    {
        // Fixed by Michael RE Broken Integration Tests (12th Feb 2013)
        // Fixed by Brendon.Page, the test now uses a mutex check to decide if a first process needs to be started.
        [TestMethod]
        public void PrepareApplication_With_ExistingApplication_Expect_OnlyOneApplication()
        {
            bool actual = false;

            var msg = string.Empty;

            try
            {
                string executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;

                var idx = executingAssemblyLocation.IndexOf(@"\TestResults", StringComparison.Ordinal);

                string studioPath = string.Empty;

                if (idx >= 0)
                {
                    string tmpPath = executingAssemblyLocation.Remove(idx);
                    studioPath = tmpPath + @"\Dev2.Studio\bin\Debug\Warewolf Studio.exe";    
                }
                
                if (!File.Exists(studioPath))
                {
                    // If this test is running in an environment this is where the Studio exe will be (otherwise this path could be resolved by getting the running server process location path)
                    studioPath = @"C:\IntegrationRun\Binaries\Warewolf Studio.exe";

                    if (!File.Exists(studioPath))
                    {
                        studioPath = @"C:\Development\Dev\Dev2.Studio\bin\Debug\Warewolf Studio.exe";
                    }
                }

                Process.Start(studioPath);

                // Wait for Process to start, and get past the check for a duplicate process
                Thread.Sleep(7000);

                // Start a second studio, this should hit the logic that checks for a duplicate and exit
                Process secondProcess = Process.Start(studioPath);

                // Gather actual
                actual = secondProcess.WaitForExit(15000);

                const string wmiQueryString = "SELECT ProcessId FROM Win32_Process WHERE Name LIKE 'Warewolf Studio%'";
                using(var searcher = new ManagementObjectSearcher(wmiQueryString))
                {
                    using(var results = searcher.Get())
                    {
                        ManagementObject mo = results.Cast<ManagementObject>().FirstOrDefault();

                        if(mo != null)
                        {
                            var id = mo.Properties["ProcessId"].Value.ToString();

                            int myID;
                            Int32.TryParse(id, out myID);

                            var proc = Process.GetProcessById(myID);

                            proc.Kill();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                msg = e.Message;
                msg += Environment.NewLine;
                msg += e.StackTrace;
            }

            Assert.IsTrue(actual, "Failed to kill second studio! [ " + msg + " ]");
        }
    }
}