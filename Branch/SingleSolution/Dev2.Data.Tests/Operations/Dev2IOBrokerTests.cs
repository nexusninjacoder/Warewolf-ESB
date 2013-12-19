﻿using System.Diagnostics.CodeAnalysis;
using Dev2.Data.Interfaces;
using Dev2.Data.Operations;
using Dev2.PathOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dev2.Data.Tests.Operations
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class Dev2IOBrokerTests
    {
        private IDev2IndexFinder _indexFinder;

        public Dev2IOBrokerTests()
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
            get { return testContextInstance; }
            set { testContextInstance = value; }
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _indexFinder = new Dev2IndexFinder();
        }

        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ActivityIOFactory_CreateOperationEndPointFromIOPath")]
        public void ActivityIOFactory_CreateOperationEndPointFromIOPath_WithSftp_ShouldSetTypeTpSFtp()
        {
            //------------Setup for test--------------------------
            const string Path = "sftp://sftp.theunlimited.co.za/text.txt";
            //------------Execute Test---------------------------
            IActivityIOOperationsEndPoint scrEndPoint = ActivityIOFactory.CreateOperationEndPointFromIOPath(ActivityIOFactory.CreatePathFromString(Path, string.Empty, null, true));
            //------------Assert Results-------------------------
            Assert.AreEqual(enActivityIOPathType.SFTP,scrEndPoint.IOPath.PathType);
        }
        
        //[TestMethod]
        //public void CopyFileWithPathsExpectedRecursiveCopy()
        //{
        //    string innerDir = Guid.NewGuid().ToString();
        //    string tempPath = Path.GetTempPath();
        //    string tempFileName = Path.GetFileName(Path.GetTempFileName());
        //    string tempData = "some string data";
        //    var tempFile = Path.Combine(tempPath, innerDir,innerDir, tempFileName);
        //    string directoryName = Path.GetDirectoryName(tempFile);
        //    Directory.CreateDirectory(directoryName);
        //    var upperLevelDir = Path.Combine(tempPath, innerDir);
        //    File.WriteAllText(tempFile,tempData);
        //    string dst = Path.Combine(tempPath, Guid.NewGuid().ToString());
        //    IActivityIOOperationsEndPoint scrEndPoint = ActivityIOFactory.CreateOperationEndPointFromIOPath(ActivityIOFactory.CreatePathFromString(upperLevelDir, string.Empty, null, true));
        //    IActivityIOOperationsEndPoint dstEndPoint = ActivityIOFactory.CreateOperationEndPointFromIOPath(ActivityIOFactory.CreatePathFromString(dst, string.Empty, null, true));

        //    var moveTO = new Dev2CRUDOperationTO(true);
        //    ActivityIOFactory.CreateOperationsBroker().Copy(scrEndPoint, dstEndPoint, moveTO);
        //    string newFilePath = Path.Combine(dst,innerDir,tempFileName);
        //    Assert.IsTrue(File.Exists(newFilePath));
        //    Assert.IsTrue(File.Exists(tempFile));
        //}
       
    }
}