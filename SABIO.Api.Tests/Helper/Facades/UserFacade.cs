using SABIO.ClientApi.Core;

namespace SABIO.Api.Tests.Helper.Facades
{
    public class UserFacade: FacadeBase
    {
        public TestUser SABIO = new TestUser("sabio", "$rfvgz/ujm*");
        public TestUser Admin = new TestUser("4nils", "sonne");
        public TestUser LeadEditor = new TestUser("3rainer", "sonne");
        public TestUser Editor = new TestUser("2anna", "sonne");

        public UserFacade(SabioClient client) : base(client)
        {}
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