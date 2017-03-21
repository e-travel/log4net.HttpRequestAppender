using System;
using NUnit.Framework;

namespace log4net.HttpRequestAppender.Tests.Unit
{
    [TestFixture]
    public class LoggingContextBaseTests
    {
        [Test]
        public void Get_a_value_which_had_been_previously_set()
        {
            var key = "key" + new Random().Next(1, 1000);
            LoggingContextBase.SetProperty(key, "value");
            var actual = LoggingContextBase.GetProperty(key);
            Assert.AreEqual("value", actual);
        }

        [Test]
        public void Get_a_value_which_wasnt_previously_set()
        {
            var key = "key" + new Random().Next(1, 1000);
            var actual = LoggingContextBase.GetProperty(key);
            Assert.IsNull(actual);
        }

        [Test]
        public void Set_to_null_and_get_value()
        {
            var key = "key" + new Random().Next(1, 1000);
            LoggingContextBase.SetProperty(key, (string) null);
            var actual = LoggingContextBase.GetProperty(key);
            Assert.IsNull(actual);
        }

        [Test]
        public void Set_then_unset_and_reset()
        {
            var key = "key" + new Random().Next(1, 1000);

            LoggingContextBase.SetProperty(key, "value1");
            Assert.AreEqual("value1", LoggingContextBase.GetProperty(key));

            LoggingContextBase.SetProperty(key, (string) null);
            Assert.IsNull(LoggingContextBase.GetProperty(key));
            
            LoggingContextBase.SetProperty(key, "value2");
            Assert.AreEqual("value2", LoggingContextBase.GetProperty(key));
        }
    }
}
