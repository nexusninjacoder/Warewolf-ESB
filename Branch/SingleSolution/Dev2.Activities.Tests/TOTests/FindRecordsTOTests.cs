﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Unlimited.Applications.BusinessDesignStudio.Activities;
// ReSharper disable InconsistentNaming
namespace Dev2.Tests.Activities.TOTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FindRecordsTOTests
    {
        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("FindRecordsTO_Constructor")]
        public void FindRecordsTO_Constructor_Default_SetsProperties()
        {
            //------------Setup for test--------------------------
            //------------Execute Test---------------------------
            var findRecordsTO = new FindRecordsTO();
            //------------Assert Results-------------------------
            Assert.IsNotNull(findRecordsTO);
            Assert.AreEqual("Match On", findRecordsTO.SearchCriteria);
            Assert.AreEqual("Equal", findRecordsTO.SearchType);
            Assert.AreEqual(0, findRecordsTO.IndexNumber);
            Assert.IsFalse(findRecordsTO.Inserted);
            Assert.IsFalse(findRecordsTO.IsSearchCriteriaEnabled);

        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("FindRecordsTO_Constructor")]
        public void FindRecordsTO_ParameterConstructor_SetsProperties()
        {
            //------------Setup for test--------------------------
            const string searchCriteria = "Bob";
            const string searchType = ">";
            const int indexNum = 3;
            //------------Execute Test---------------------------
            var findRecordsTO = new FindRecordsTO(searchCriteria, searchType, indexNum);
            //------------Assert Results-------------------------
            Assert.IsNotNull(findRecordsTO);
            Assert.AreEqual(searchCriteria, findRecordsTO.SearchCriteria);
            Assert.AreEqual(searchType, findRecordsTO.SearchType);
            Assert.AreEqual(indexNum, findRecordsTO.IndexNumber);
            Assert.IsFalse(findRecordsTO.Inserted);
            Assert.IsFalse(findRecordsTO.IsSearchCriteriaEnabled);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("FindRecordsTO_SearchType")]
        public void FindRecordsTO_SearchType_SetValue_FiresNotifyPropertyChanged()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO();
            const string searchType = "MyValue";
            //------------Execute Test---------------------------
            var notifyPropertyChanged = TestUtils.PropertyChangedTester(findRecordsTO, () => findRecordsTO.SearchType, () => findRecordsTO.SearchType = searchType);
            //------------Assert Results-------------------------
            Assert.AreEqual(searchType, findRecordsTO.SearchType);
            Assert.IsTrue(notifyPropertyChanged);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("FindRecordsTO_SearchType")]
        public void FindRecordsTO_SearchCriteria_SetValue_FiresNotifyPropertyChanged()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO();
            const string searchCriteria = "MyValue";
            //------------Execute Test---------------------------
            var notifyPropertyChanged = TestUtils.PropertyChangedTester(findRecordsTO, () => findRecordsTO.SearchCriteria, () => findRecordsTO.SearchCriteria = searchCriteria);
            //------------Assert Results-------------------------
            Assert.AreEqual(searchCriteria, findRecordsTO.SearchCriteria);
            Assert.IsTrue(notifyPropertyChanged);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("FindRecordsTO_IndexNum")]
        public void FindRecordsTO_IndexNum_SetValue_FiresNotifyPropertyChanged()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO();
            const int indexNum = 5;
            //------------Execute Test---------------------------
            var notifyPropertyChanged = TestUtils.PropertyChangedTester(findRecordsTO, () => findRecordsTO.IndexNumber, () => findRecordsTO.IndexNumber = indexNum);
            //------------Assert Results-------------------------
            Assert.AreEqual(indexNum, findRecordsTO.IndexNumber);
            Assert.IsTrue(notifyPropertyChanged);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("FindRecordsTO_IsSearchCriteriaEnabled")]
        public void FindRecordsTO_IsSearchCriteriaEnabled_SetValue_FiresNotifyPropertyChanged()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO();
            const bool isSearchCriteriaEnabled = true;
            //------------Execute Test---------------------------
            var notifyPropertyChanged = TestUtils.PropertyChangedTester(findRecordsTO, () => findRecordsTO.IsSearchCriteriaEnabled, () => findRecordsTO.IsSearchCriteriaEnabled = isSearchCriteriaEnabled);
            //------------Assert Results-------------------------
            Assert.AreEqual(isSearchCriteriaEnabled, findRecordsTO.IsSearchCriteriaEnabled);
            Assert.IsTrue(notifyPropertyChanged);
        }

        #region CanAdd Tests

        [TestMethod]
        [Owner("Massimo Guerrera")]
        [TestCategory("FindRecordsTO_CanAdd")]
        public void FindRecordsTO_CanAdd_SearchTypeEmpty_ReturnFalse()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO() {SearchType = string.Empty};            
            //------------Execute Test---------------------------
            Assert.IsFalse(findRecordsTO.CanAdd());
            //------------Assert Results-------------------------
        }

        [TestMethod]
        [Owner("Massimo Guerrera")]
        [TestCategory("FindRecordsTO_CanAdd")]
        public void FindRecordsTO_CanAdd_SearchTypeWithData_ReturnTrue()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO() { SearchType = "Contains" };
            //------------Execute Test---------------------------
            Assert.IsTrue(findRecordsTO.CanAdd());
            //------------Assert Results-------------------------
        }

        #endregion

        #region CanRemove Tests

        [TestMethod]
        [Owner("Massimo Guerrera")]
        [TestCategory("FindRecordsTO_CanRemove")]
        public void FindRecordsTO_CanRemove_SearchTypeEmptyAndSearchTypeEmpty_ReturnTrue()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO() { SearchType = string.Empty,SearchCriteria = string.Empty};
            //------------Execute Test---------------------------
            Assert.IsTrue(findRecordsTO.CanRemove());
            //------------Assert Results-------------------------
        }

        [TestMethod]
        [Owner("Massimo Guerrera")]
        [TestCategory("FindRecordsTO_CanRemove")]
        public void FindRecordsTO_CanRemove_SearchTypeWithDataAndSearchTypeEmpty_ReturnFalse()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO() { SearchType = "Contains", SearchCriteria = string.Empty };
            //------------Execute Test---------------------------
            Assert.IsFalse(findRecordsTO.CanRemove());
            //------------Assert Results-------------------------
        }

        [TestMethod]
        [Owner("Massimo Guerrera")]
        [TestCategory("FindRecordsTO_CanRemove")]
        public void FindRecordsTO_CanRemove_SearchTypeEmptyAndSearchTypeWithData_ReturnFalse()
        {
            //------------Setup for test--------------------------
            var findRecordsTO = new FindRecordsTO() { SearchType = string.Empty, SearchCriteria = "Data" };
            //------------Execute Test---------------------------
            Assert.IsFalse(findRecordsTO.CanRemove());
            //------------Assert Results-------------------------
        }

        #endregion
    }
}