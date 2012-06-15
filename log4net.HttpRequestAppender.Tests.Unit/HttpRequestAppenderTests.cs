using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Tests.Appender;
using log4net.Util;
using Moq;
using NUnit.Framework;

namespace log4net
{
    [TestFixture]
    public class HttpRequestAppenderTests
    {
        private Mock<HttpRequestAppender> _httpRequestAppenderMock;
        private HttpRequestAppender _httpRequestAppender;


        private CountingAppender _countingAppender;
        private Repository.Hierarchy.Hierarchy _hierarchy;
        private Mock<IContextManager> _contextManagerMock;

        [SetUp]
        public void SetupRepository()
        {
            _hierarchy = new Repository.Hierarchy.Hierarchy();

            _countingAppender = new CountingAppender();
            _countingAppender.ActivateOptions();

            _contextManagerMock = new Mock<IContextManager>();
            var contextMock = new Mock<Context>(It.IsAny<object>());
            contextMock.SetupGet(c => c.Events)
                   .Returns((GroupedEvents) null)
                   .Verifiable();
            _contextManagerMock.Setup(c => c.BuildContext())
                               .Returns(contextMock.Object)
                               .Verifiable();

            _httpRequestAppenderMock = new Mock<HttpRequestAppender> { CallBase = true };
            _httpRequestAppenderMock.SetupGet(a => a.DoesRequireLayout).Returns(false).Verifiable();
            _httpRequestAppender = _httpRequestAppenderMock.Object;
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

        /// <summary>
        /// </summary>
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
