using System;
using FluentAssertions;
using SafeBunny.Core.Publishing.Continuation;
using Xunit;

namespace SafeBunny.Core.Tests
{
    public sealed class ContinuationTests : SafeBunnyTest
    {
        private readonly ICorrelationContinuation Unit;

        public ContinuationTests()
        {
            Unit = new CorrelationContinuation();
        }

        [Fact]
        public void When_MarkerIs1_ShouldSetLastCharacter()
        {
            var guid = Guid.NewGuid().ToString("N");
            var expected = new Span<char>(guid.ToCharArray())
            {
                [guid.Length-1] = 'b',
                [guid.Length - 2] = 'd'
            };

            var result = Unit.Continue(guid, 75);

            result.Should().BeEquivalentTo(expected.ToString());
        }

        protected override void CreateMocks()
        { }

    }
}
