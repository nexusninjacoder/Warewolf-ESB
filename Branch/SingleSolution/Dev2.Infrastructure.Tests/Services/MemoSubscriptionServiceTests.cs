﻿using System;
using System.Diagnostics.CodeAnalysis;
using Dev2.Communication;
using Dev2.Providers.Events;
using Dev2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dev2.Infrastructure.Tests.Services
{
    [TestClass][ExcludeFromCodeCoverage]
    public class MemoSubscriptionServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MemoSubscriptionServiceConstructorWithNullServiceArgsExpectedThrowsArgumentNullException()
        {
            var memoService = new MemoSubscriptionService<Memo>((SubscriptionService<Memo>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MemoSubscriptionServiceConstructorWithNullPublisherArgsExpectedThrowsArgumentNullException()
        {
            var memoService = new MemoSubscriptionService<Memo>((IEventPublisher)null);
        }

        [TestMethod]
        public void MemoSubscriptionServiceConstructorWithValidArgsExpectedDoesNotThrowException()
        {
            var publisher = new Mock<IEventPublisher>();
            var memoService = new MemoSubscriptionService<Memo>(publisher.Object);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void MemoSubscriptionServiceSubscribeWithArgsExpectedAddsSubscriptionFilteredByMemoID()
        {
            Func<Memo, bool> actualFilter = null;

            var service = new Mock<ISubscriptionService<Memo>>();
            service.Setup(s => s.Subscribe(It.IsAny<Func<Memo, bool>>(), It.IsAny<Action<Memo>>()))
                   .Callback((Func<Memo, bool> filter, Action<Memo> onNext) => actualFilter = filter)
                   .Verifiable();

            var instanceID = Guid.NewGuid();
            var memoService = new MemoSubscriptionService<Memo>(service.Object);
            memoService.Subscribe(instanceID, memo => { });

            service.Verify(s => s.Subscribe(It.IsAny<Func<Memo, bool>>(), It.IsAny<Action<Memo>>()), Times.Once());

            var posResult = actualFilter.Invoke(new Memo { InstanceID = instanceID });
            Assert.IsTrue(posResult);

            var negResult = actualFilter.Invoke(new Memo { InstanceID = new Guid() });
            Assert.IsFalse(negResult);
        }

        [TestMethod]
        public void MemoSubscriptionServiceDisposeExpectedDisposesManagedObjects()
        {
            var service = new Mock<ISubscriptionService<Memo>>();
            service.Setup(s => s.Dispose()).Verifiable();

            var memoService = new MemoSubscriptionService<Memo>(service.Object);

            memoService.Dispose();

            service.Verify(s => s.Dispose());
        }
    }
}