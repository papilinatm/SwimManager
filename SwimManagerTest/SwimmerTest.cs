using SwimManager;

namespace SwimManagerTest
{
    public partial class Tests
    {
        [Test]
        public void SetNameTest()
        {
            var s1 = new Swimmer()
            {
                Name = "Ivan   Ivanov "
            };
            Assert.IsTrue(s1.Name=="Ivan Ivanov");
        }
        [Test]
        public void IsEqualTest()
        {
            var s1 = new Swimmer()
            {
                Name = "Ivanov Ivan   Ivanovich "
            };
            var s2 = new Swimmer()
            {
                Name = "Ivanov Ivan",
                Gender = Gender.Male
            };
            Assert.IsTrue(s1==s2);
        }
        [Test]
        public void MergeTest()
        {
            var dt = DateTime.Now;
            var s1 = new Swimmer()
            {
                Name = "Ivanov Ivan   Ivanovich ",
                AllResults = [new Result( Style.Butterfly, 50, TimeSpan.FromSeconds(50), DateTime.Now),
                              new Result(Style.Butterfly, 25, TimeSpan.FromSeconds(50), dt)]
            };
            var s2 = new Swimmer()
            {
                Name = "Ivanov Ivan",
                Gender = Gender.Male,
                Year = 2000,
                AllResults = [
                    new Result(Style.Breaststroke, 50, TimeSpan.FromSeconds(50), DateTime.Now),
                    new Result(Style.Butterfly, 25, TimeSpan.FromSeconds(50), dt),
                    new Result(Style.Butterfly, 50, TimeSpan.FromSeconds(45), DateTime.Now)
                ]
            };
            Swimmer.MergeSwimmers(s1, s2);
            Assert.IsTrue(s1.Name=="Ivanov Ivan Ivanovich"
                && s1.Year==2000
                && s1.Gender == Gender.Male
                && s1.AllResults.Count==4);
        }
    }
}