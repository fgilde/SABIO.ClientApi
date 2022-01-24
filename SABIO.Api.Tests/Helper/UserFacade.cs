namespace SABIO.Api.Tests.Helper
{
    public class UserFacade
    {
        public TestUser SABIO = new TestUser("sabio", "$rfvgz/ujm*");
        public TestUser Admin = new TestUser("4nils", "sonne");
        public TestUser LeadEditor = new TestUser("3rainer", "sonne");
        public TestUser Editor = new TestUser("2anna", "sonne");
    }

    public class TestUser
    {
        public TestUser(string name = "", string password = "")
        {
            Name = name;
            Password = password;
        }

        public string Name { get; set; }
        public string Password { get; set; }
    }
}