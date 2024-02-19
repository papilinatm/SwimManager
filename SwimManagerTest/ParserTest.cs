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
            ExportImport.ImportSwimmersFromApplicationList(@"data\xlsx\test.xlsx");
            Assert.Pass();
        }
    }
}