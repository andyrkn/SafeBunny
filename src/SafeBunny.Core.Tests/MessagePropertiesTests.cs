using System;
using FluentAssertions;
using SafeBunny.Core.Message;
using Xunit;

namespace SafeBunny.Core.Tests
{
    public sealed class MessagePropertiesTests : SafeBunnyTest
    {
        private readonly MessageProperties Unit;

        public MessagePropertiesTests()
        {
            Unit = new MessageProperties();
        }

        [Fact]
        public void Given_Continuation_ShouldReturnFromHeaders()
        {
            Unit.ContinuationMarker.Should().Be(0);
        }

        [Fact]
        public void Given_Continuation_ShouldReturnCorrectValue()
        {
            Unit.ContinuationMarker = Unit.ContinuationMarker + 1;
            Unit.ContinuationMarker.Should().Be(1);
        }

        [Fact]
        public void Given_RetryAttempt_ShouldReturnCorrectValue()
        {
            Unit.RetryAttempt = Unit.RetryAttempt + 2;
            Unit.RetryAttempt.Should().Be(2);
        }

        [Fact]
        public void Given_DeliveryDelay_ShouldReturnCorrectValue()
        {
            Unit.DeliveryDelay = TimeSpan.FromMilliseconds(100);
            Unit.DeliveryDelay.Should().Be(TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void Given_NoDeliveryDelay_ShouldReturnCorrectValue()
        {
            Unit.DeliveryDelay.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void Given_ReplyTo_ShouldReturnCorrectValue()
        {
            var expected = "reply-to-queue";
            Unit.ReplyTo = expected;
            Unit.ReplyTo.Should().Be(expected);
        }

        [Fact]
        public void Given_Continue_ShouldHaveSeparateReferences()
        {
            var continuation = Unit.Continue();
            continuation.Should().BeEquivalentTo(Unit);

            continuation.ContinuationMarker += 1;
            continuation.DeliveryDelay = TimeSpan.MinValue;
            continuation.CorrelationId = Guid.NewGuid().ToString();
            continuation.ReplyTo = "Hehe";
            continuation.ContinuationMarker.Should().NotBe(Unit.ContinuationMarker);
            continuation.DeliveryDelay.Should().NotBe(Unit.DeliveryDelay);
            continuation.CorrelationId.Should().NotBeEquivalentTo(Unit.CorrelationId);
            continuation.ReplyTo.Should().NotBeEquivalentTo(Unit.ReplyTo);
        }

        protected override void CreateMocks()
        { }
    }
}