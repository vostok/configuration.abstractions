using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.Extensions;

namespace Vostok.Configuration.Abstractions.Tests
{
    [TestFixture]
    internal class ObservableExtensions_Tests
    {
        private IObservable<int> numbers;
        private IObserver<int> observer;
        private List<int> received;

        [SetUp]
        public void TestSetup()
        {
            received = new List<int>();

            numbers = Substitute.For<IObservable<int>>();
            numbers.Subscribe(Arg.Do<IObserver<int>>(o => observer = o));
        }

        [Test]
        public void Subscribe_with_OnNext_should_call_provided_action_on_next_item()
        {
            numbers.Subscribe(i => received.Add(i));

            observer.OnNext(1);
            observer.OnNext(2);

            received.Should().Equal(1, 2);
        }


        [Test]
        public void Subscribe_with_OnNext_should_throw_on_error()
        {
            numbers.Subscribe(i => { });

            new Action(() => observer.OnError(new Exception("test"))).Should().Throw<Exception>();
        }

        [Test]
        public void Subscribe_with_OnNext_and_OnError_should_call_provided_action_on_next_item()
        {
            numbers.Subscribe(i => received.Add(i), e => { });

            observer.OnNext(1);
            observer.OnNext(2);

            received.Should().Equal(1, 2);
        }

        [Test]
        public void Subscribe_with_OnNext_and_OnError_should_call_provided_action_on_error()
        {
            Exception error = null;
            numbers.Subscribe(i => { }, e => error = e);

            var expectedError = new Exception();
            observer.OnError(expectedError);

            error.Should().BeSameAs(expectedError);
        }
    }
}
