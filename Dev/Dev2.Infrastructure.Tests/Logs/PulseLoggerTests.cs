﻿using System.Threading;
using System.Timers;
using Dev2.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev2.Infrastructure.Tests.Logs
{
    [TestClass]
    public class PulseLoggerTests
    {
        bool _elapsed;
        [TestMethod]
        [Owner("Leon Rajindrapersadh")]
        [TestCategory("PulseLogger_Ctor")]
// ReSharper disable InconsistentNaming
        public void PulseLogger_Ctor_CheckValues_ExpectInitialised()
// ReSharper restore InconsistentNaming
        {
            //------------Setup for test--------------------------
            var pulseLogger = new PulseLogger(25);
            Assert.AreEqual(pulseLogger.Interval,25);
            PrivateObject pvt = new PrivateObject(pulseLogger);
            System.Timers.Timer timer = (System.Timers.Timer)pvt.GetField("_timer");
            Assert.AreEqual(false,timer.Enabled);
            
            //------------Execute Test---------------------------

            //------------Assert Results-------------------------
        }

        [TestMethod]
        [Owner("Leon Rajindrapersadh")]
        [TestCategory("PulseLogger_Ctor")]
// ReSharper disable InconsistentNaming
        public void PulseLogger_Ctor_Start_ExpectInitialised()
// ReSharper restore InconsistentNaming
        {
            //------------Setup for test--------------------------
            var pulseLogger = new PulseLogger(2500);
            
            Assert.AreEqual(pulseLogger.Interval, 2500);
            PrivateObject pvt = new PrivateObject(pulseLogger);
            System.Timers.Timer timer = (System.Timers.Timer)pvt.GetField("_timer");
            timer.Elapsed += TimerElapsed;
            Assert.AreEqual(false, timer.Enabled);
            pulseLogger.Start();
            Thread.Sleep(3000);
            //------------Execute Test---------------------------
            Assert.IsTrue(_elapsed);
            //------------Assert Results-------------------------
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _elapsed = true;

        }
    }
}