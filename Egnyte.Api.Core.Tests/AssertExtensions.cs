using System;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests
{
    using NUnit.Framework;

    public static class AssertExtensions
    {
        public static async Task<TException> ThrowsAsync<TException>(Func<Task> func) where TException : class
        {
            var exception = default(TException);
            var expected = typeof(TException);
            Type actual = null;
            try
            {
                await func();
            }
            catch (Exception e)
            {
                exception = e as TException;
                actual = e.GetType();
            }

            Assert.AreEqual(expected, actual);
            
            return exception;
        }
    }
}
