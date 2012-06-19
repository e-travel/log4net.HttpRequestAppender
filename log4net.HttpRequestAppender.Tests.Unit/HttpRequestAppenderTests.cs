using log4net.Config;
using log4net.Core;
using log4net.HttpRequestAppender.Tests.Unit.Appender;
using NUnit.Framework;

namespace log4net.HttpRequestAppender.Tests.Unit
{
    internal class HttpRequestAppenderStub : log4net.Appender.HttpRequestAppender
    {
        protected override bool RequiresLayout { get { return false; } }
    }

    [TestFixture]
    public class HttpRequestAppenderTests
    {
        private log4net.Appender.HttpRequestAppender _httpRequestAppender;
        private CountingAppender _countingAppender;
        private Repository.Hierarchy.Hierarchy _hierarchy;

        [SetUp]
        public void SetupRepository()
        {
            _hierarchy = new Repository.Hierarchy.Hierarchy();

            _countingAppender = new CountingAppender();
            _countingAppender.ActivateOptions();

            _httpRequestAppender = new HttpRequestAppenderStub();
            _httpRequestAppender.AddAppender(_countingAppender);

            _httpRequestAppender.ClearFilters();
            _httpRequestAppender.Threshold = Level.All;

            _httpRequestAppender.ActivateOptions();

            BasicConfigurator.Configure(_hierarchy, _httpRequestAppender);
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual(0, _countingAppender.Counter, "Test empty appender");

            var logger = _hierarchy.GetLogger("test");
            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message logged", null);

            Assert.AreEqual(1, _countingAppender.Counter);
        }

        [Test]
        public void TestBufferSize5()
        {
            _httpRequestAppender.ActivateOptions();

            Assert.AreEqual(_countingAppender.Counter, 0);

            var logger = _hierarchy.GetLogger("test");

            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message 1", null);
            Assert.AreEqual(1, _countingAppender.Counter);
            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message 2", null);
            Assert.AreEqual(2, _countingAppender.Counter);
            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message 3", null);
            Assert.AreEqual(3, _countingAppender.Counter);
            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message 4", null);
            Assert.AreEqual(4, _countingAppender.Counter);
            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message 5", null);
            Assert.AreEqual(5, _countingAppender.Counter);
            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message 6", null);
            Assert.AreEqual(6, _countingAppender.Counter);
            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message 7", null);
            Assert.AreEqual(7, _countingAppender.Counter);
            logger.Log(typeof(HttpRequestAppenderTests), Level.Warn, "Message 8", null);
            Assert.AreEqual(8, _countingAppender.Counter);
        }
    }
}
