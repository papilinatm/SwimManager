using SwimManager;

namespace SwimManagerTest
{
    public partial class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParseApplicationsFromXLSXTest()
        {
            Parser.ParseApplicationsFromXLSX();
            Assert.Pass();
        }
    }
}