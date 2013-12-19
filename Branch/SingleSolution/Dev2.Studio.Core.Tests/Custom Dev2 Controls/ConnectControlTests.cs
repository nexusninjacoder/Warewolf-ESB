﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using Caliburn.Micro;
using Dev2.Composition;
using Dev2.Core.Tests.Environments;
using Dev2.Core.Tests.Utils;
using Dev2.Messages;
using Dev2.Studio.Core;
using Dev2.Studio.Core.Interfaces;
using Dev2.Studio.Core.Messages;
using Dev2.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

// ReSharper disable InconsistentNaming
namespace Dev2.Core.Tests.Custom_Dev2_Controls
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ConnectControlTests
    {
        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_SelectionChanged")]
        public void ConnectControl_SelectionChanged_WhenHasItem_NoViewModel_ShouldNotFireMessages()
        {
            //------------Setup for test--------------------------
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var mockEventAggregator = new Mock<IEventAggregator>();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>())).Verifiable();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>())).Verifiable();
            var syncWorker = AsyncWorkerTests.CreateSynchronousAsyncWorker();
            var connectControl = new ConnectControl(mockEventAggregator.Object, syncWorker.Object);
            connectControl.TheServerComboBox.ItemsSource = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            //------------Execute Test---------------------------
            connectControl.TheServerComboBox.SelectedItem = remoteServer;
            //------------Assert Results-------------------------
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>()), Times.Never());
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>()), Times.Never());
        } 
        
        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_SelectionChanged")]
        public void ConnectControl_SelectionChanged_WhenHasItem_ViewModelIsSelectedFromDropDownFalse_ShouldNotFireMessages_IsEditEnabledTrue()
        {
            //------------Setup for test--------------------------
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var mockEventAggregator = new Mock<IEventAggregator>();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>())).Verifiable();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>())).Verifiable();
            var syncWorker = AsyncWorkerTests.CreateSynchronousAsyncWorker();
            var connectControl = new ConnectControl(mockEventAggregator.Object,syncWorker.Object);
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.SelectedServer = remoteServer;
            connectControlViewModel.IsSelectedFromDropDown = false;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            //------------Execute Test---------------------------
            connectControl.TheServerComboBox.SelectedItem = remoteServer;
            connectControl.SelectionHasChanged(remoteServer);
            //------------Assert Results-------------------------
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>()), Times.Never());
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>()), Times.Never());
            Assert.IsTrue(connectControl.ViewModel.IsEditEnabled.GetValueOrDefault(false));
        }  

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_SelectionChanged")]
        public void ConnectControl_SelectionChanged_WhenHasItem_ViewModelIsSelectedFromDropDownFalse_ShouldNotFireMessages_IsEditEnabledFalse()
        {
            //------------Setup for test--------------------------
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var mockEventAggregator = new Mock<IEventAggregator>();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>())).Verifiable();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>())).Verifiable();
            var syncWorker = AsyncWorkerTests.CreateSynchronousAsyncWorker();
            var connectControl = new ConnectControl(mockEventAggregator.Object,syncWorker.Object);
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.SelectedServer = localhostServer;
            connectControlViewModel.IsSelectedFromDropDown = false;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            //------------Execute Test---------------------------
            connectControl.TheServerComboBox.SelectedItem = localhostServer;
            connectControl.SelectionHasChanged(localhostServer);
            //------------Assert Results-------------------------
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>()), Times.Never());
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>()), Times.Never());
            Assert.IsFalse(connectControl.ViewModel.IsEditEnabled.GetValueOrDefault(true));
        }   

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_SelectionChanged")]
        public void ConnectControl_SelectionChanged_WhenHasItemNullViewModelSelectedServer_ViewModelIsSelectedFromDropDownFalse_ShouldNotFireMessages_IsEditEnabledFalse()
        {
            //------------Setup for test--------------------------
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var mockEventAggregator = new Mock<IEventAggregator>();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>())).Verifiable();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>())).Verifiable();
            var syncWorker = AsyncWorkerTests.CreateSynchronousAsyncWorker();
            var connectControl = new ConnectControl(mockEventAggregator.Object,syncWorker.Object);
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.SelectedServer = null;
            connectControlViewModel.IsSelectedFromDropDown = false;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            //------------Execute Test---------------------------
            connectControl.TheServerComboBox.SelectedItem = localhostServer;
            connectControl.SelectionHasChanged(localhostServer);
            //------------Assert Results-------------------------
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>()), Times.Never());
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>()), Times.Never());
            Assert.IsFalse(connectControl.ViewModel.IsEditEnabled.GetValueOrDefault(true));
        }     
   
        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_SelectionChanged")]
        public void ConnectControl_SelectionChanged_WhenHasItem_ViewModelIsSelectedFromDropDownTrue_ShouldFireMessages()
        {
            //------------Setup for test--------------------------
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var mockEventAggregator = new Mock<IEventAggregator>();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>())).Verifiable();
            mockEventAggregator.Setup(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>())).Verifiable();
            var syncWorker = AsyncWorkerTests.CreateSynchronousAsyncWorker();
            var connectControl = new ConnectControl(mockEventAggregator.Object,syncWorker.Object);
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.IsSelectedFromDropDown = true;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            //------------Execute Test---------------------------
            connectControl.TheServerComboBox.SelectedItem = connectControl.ViewModel.Servers[1];
            connectControlViewModel.SelectedServer = remoteServer;
            connectControl.SelectionHasChanged(connectControl.ViewModel.Servers[1]);
            //------------Assert Results-------------------------
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetSelectedItemInExplorerTree>()), Times.Once());
            mockEventAggregator.Verify(aggregator => aggregator.Publish(It.IsAny<SetActiveEnvironmentMessage>()), Times.Once());
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_HandleUpdateSelectedServer")]
        public void ConnectControl_HandleUpdateSelectedServer_WithSourceTrueLabelTextConnect_DoesNotUpdateSelectedServer()
        {
            //------------Setup for test--------------------------
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var connectControl = new ConnectControl();
            connectControl.LabelText = "Connect";
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.IsSelectedFromDropDown = true;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            connectControl.TheServerComboBox.SelectedItem = connectControl.ViewModel.Servers[0];
            connectControlViewModel.SelectedServer = localhostServer;
            var updateSelectedServerMessage = new UpdateSelectedServer(remoteServer, true);
            //------------Execute Test---------------------------
            connectControl.Handle(updateSelectedServerMessage);
            //------------Assert Results-------------------------
            Assert.AreEqual(localhostServer, connectControlViewModel.SelectedServer);
        }
        
        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_HandleUpdateSelectedServer")]
        public void ConnectControl_HandleUpdateSelectedServer_WithSourceTrueLabelTextDestination_DoesNotUpdateSelectedServer()
        {
            //------------Setup for test--------------------------
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var connectControl = new ConnectControl();
            connectControl.LabelText = "Destination Server";
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.IsSelectedFromDropDown = true;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            connectControl.TheServerComboBox.SelectedItem = connectControl.ViewModel.Servers[0];
            connectControlViewModel.SelectedServer = localhostServer;
            var updateSelectedServerMessage = new UpdateSelectedServer(remoteServer, true);
            //------------Execute Test---------------------------
            connectControl.Handle(updateSelectedServerMessage);
            //------------Assert Results-------------------------
            Assert.AreEqual(localhostServer, connectControlViewModel.SelectedServer);
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_HandleUpdateSelectedServer")]
        public void ConnectControl_HandleUpdateSelectedServer_WithSourceTrueLabelTextSource_UpdateSelectedServer()
        {
            //------------Setup for test--------------------------
            SetupMef();
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var mockEnvironmentRepository = new TestEnvironmentRespository(localhostServer, remoteServer, otherServer);
            new EnvironmentRepository(mockEnvironmentRepository);
            var connectControl = new ConnectControl();
            connectControl.LabelText = "Source Server";
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.IsSelectedFromDropDown = true;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            connectControl.TheServerComboBox.SelectedItem = connectControl.ViewModel.Servers[0];
            connectControlViewModel.SelectedServer = localhostServer;
            var updateSelectedServerMessage = new UpdateSelectedServer(remoteServer, true);
            //------------Execute Test---------------------------
            connectControl.Handle(updateSelectedServerMessage);
            //------------Assert Results-------------------------
            Assert.AreNotEqual(localhostServer, connectControlViewModel.SelectedServer);
            Assert.AreEqual(remoteServer.Name, connectControlViewModel.SelectedServer.Name);
        }


        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_HandleUpdateSelectedServer")]
        public void ConnectControl_HandleUpdateSelectedServer_WithSourceFalseLabelTextDestination_UpdateSelectedServer()
        {
            //------------Setup for test--------------------------
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var connectControl = new ConnectControl();
            connectControl.LabelText = "Destination Server";
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.IsSelectedFromDropDown = true;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            connectControl.TheServerComboBox.SelectedItem = connectControl.ViewModel.Servers[0];
            connectControlViewModel.SelectedServer = localhostServer;
            var updateSelectedServerMessage = new UpdateSelectedServer(remoteServer, false);
            //------------Execute Test---------------------------
            connectControl.Handle(updateSelectedServerMessage);
            //------------Assert Results-------------------------
            Assert.AreNotEqual(localhostServer, connectControlViewModel.SelectedServer);
            Assert.AreEqual(remoteServer.Name, connectControlViewModel.SelectedServer.Name);
            
        }

        [TestMethod]
        [Owner("Hagashen Naidu")]
        [TestCategory("ConnectControl_HandleUpdateSelectedServer")]
        public void ConnectControl_HandleUpdateSelectedServer_WithDestinationFalseLabelTextSource_DoesNotUpdateSelectedServer()
        {
            //------------Setup for test--------------------------
            SetupMef();
            var localhostServer = CreateServer("localhost", true);
            var remoteServer = CreateServer("remote", false);
            var otherServer = CreateServer("disconnected", false);
            var mockEnvironmentRepository = new TestEnvironmentRespository(localhostServer, remoteServer, otherServer);
            new EnvironmentRepository(mockEnvironmentRepository);
            var connectControl = new ConnectControl();
            connectControl.LabelText = "Source Server";
            var connectControlViewModel = new ConnectControlViewModel(localhostServer);
            connectControlViewModel.IsSelectedFromDropDown = true;
            var serverDtos = new List<IEnvironmentModel> { localhostServer, remoteServer, otherServer };
            connectControl.ViewModel = connectControlViewModel;
            var observableCollection = new ObservableCollection<IEnvironmentModel>(serverDtos);
            connectControl.ViewModel.Servers = observableCollection;
            connectControl.TheServerComboBox.ItemsSource = connectControl.ViewModel.Servers;
            connectControl.TheServerComboBox.SelectedItem = connectControl.ViewModel.Servers[0];
            connectControlViewModel.SelectedServer = localhostServer;
            var updateSelectedServerMessage = new UpdateSelectedServer(remoteServer, false);
            //------------Execute Test---------------------------
            connectControl.Handle(updateSelectedServerMessage);
            //------------Assert Results-------------------------
            Assert.AreEqual(localhostServer, connectControlViewModel.SelectedServer);
        }

        public static ImportServiceContext EnviromentRepositoryImportServiceContext;
        static void SetupMef()
        {

            var eventAggregator = new Mock<IEventAggregator>();

            var importServiceContext = new ImportServiceContext();
            ImportService.CurrentContext = importServiceContext;
            ImportService.Initialize(new List<ComposablePartCatalog>());
            ImportService.AddExportedValueToContainer(eventAggregator.Object);

            EnviromentRepositoryImportServiceContext = importServiceContext;
        }

        static IEnvironmentModel CreateServer(string name, bool isConnected)
        {
            var isLocalhost = name == "localhost";

            var env = new Mock<IEnvironmentModel>();
            env.Setup(e => e.Name).Returns(name);
            env.Setup(e => e.ID).Returns(isLocalhost ? Guid.Empty : Guid.NewGuid());
            env.Setup(e => e.IsConnected).Returns(isConnected);
            env.Setup(e => e.IsLocalHost()).Returns(isLocalhost);

            return env.Object;
        }
    }
}