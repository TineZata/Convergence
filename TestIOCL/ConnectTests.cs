using IOCL;

namespace TestIOCL
{
    public class Tests
    {
        private IIOConvergenceApi api; 
        [SetUp]
        public void Setup()
        {
            api = new IOConvergenceApiImp();
        }

        [Test]
        public void Connect_ValidParameters_ReturnsSessionId()
        {
            var settings = new EpicsCaSettings();
            var args = new EndPointBase<EpicsCaSettings> { Settings = settings };
        }
    }
}