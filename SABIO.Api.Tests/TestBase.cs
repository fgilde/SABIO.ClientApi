using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SABIO.Api.Tests.Helper;
using SABIO.Api.Tests.Helper.Facades;
using SABIO.ClientApi.Core;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.Api.Tests
{
    public class TestBase<TApi> : TestBase where TApi : SabioApiBase, new()
    {
        public TApi Api => TestClient.Api<TApi>(); 
    }

    public class TestBase
    {
        public TestBase()
        {
            Facade = new TestFacade(TestClient);
        }

        private SabioClient client;

        protected TestFacade Facade;

        protected SabioClient TestClient => client ??= SabioClient.CreateAsync(TestFacade.SabioUrl).Result;

        protected SabioClient AuthenticateClient(TestUser user = null)
        {
            user ??= Facade.Users.Editor;
            TestClient.Login(user);
            return TestClient;
        }


        [TestInitialize]
        public void TestInitialize()
        {
            AuthenticateClient(Facade.Users.Admin);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestClient.Apis.Authentication.LogoutAsync().Wait();
        }

        protected TextResult CreateTextResource(string title, string content)
        {
            var tree = TestClient.Api<TreeApi>().TreeAsync().Result;

            var nodes = new[]
            {
                tree.Data.Result.Children.First().Children.First().Children.First(),
                tree.Data.Result.Children[2].Children.First().Children.First()
            };
            User user = TestClient.Apis.Authentication.GetCurrentUserAsync().Result;
            Text textToCreate = new Text
            {
                Title = title,
                Paths = nodes.ToPathsArray(),
                Branches = nodes.GetUniqueBranches().ToArray(),
                Fragments = new[]
                {
                    new Fragment {
                        Content = content,
                        Branches = nodes.GetUniqueBranches().ToArray()
                    }
                },
                CreatedBy = user,
                Group = user.Groups.First(g => g.Name == "Leiter Redaktion")
                //Group = user.Groups.First(g => g.Name == "Atlantis GER Agent") // Das sabio BE ist so kacke, dass man bei dieser Gruppe z.B gar keinen Response mehr bekommt, der server also irgendwo direkt austeigt

            };

            // Call manually Post to keep cache. otherwise the default behavior for this is  TestClient.Apis.Texts.CreateAsync(textToCreate).Result;
            return TestClient.PostAsync<SabioResponse<TextResult>>($"{TestClient.Apis.Texts.ApiPath.Path}", textToCreate).Result.Data;
        }
    }
}