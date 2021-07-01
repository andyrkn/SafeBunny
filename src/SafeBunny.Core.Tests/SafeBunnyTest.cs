using System;
using Moq;

namespace SafeBunny.Core.Tests
{
    public abstract class SafeBunnyTest : IDisposable
    {
        protected readonly MockRepository Repository;

        protected SafeBunnyTest()
        {
            Repository = new MockRepository(MockBehavior.Strict);
            CreateMocks();
        }

        protected abstract void CreateMocks();

        public void Dispose()
        {
            Repository.VerifyAll();
        }
    }
}