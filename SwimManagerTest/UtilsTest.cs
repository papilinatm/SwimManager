using SwimManager;

namespace SwimManagerTest
{
     public partial class Tests
    {
        [Test]
        public void LoadNamesTest()
        {
            Assert.IsTrue(Data.GetGenderByName("Татьяна") == Gender.Female);
        }
    }
}