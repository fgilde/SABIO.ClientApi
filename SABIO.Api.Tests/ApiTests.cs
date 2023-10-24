using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SABIO.Api.Tests.Helper;
using SABIO.ClientApi;
using SABIO.ClientApi.Core;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Responses.Types;
using SABIO.ClientApi.Types;

namespace SABIO.Api.Tests
{
    [TestClass]
    public class ApiBaseTests : TestBase
    {
        [TestMethod]
        public void CanSerializeGetParams()
        {
            var query = new SearchQuery("test");
            query.SortBy(r => r.Title, SortDirection.Ascending);
            string s = query.ToQueryString();
        }

        [TestMethod]
        public void ApisCanWork()
        {
            var c = TestClient;
            var con = c.Api<ConfigApi>().ClientConfigAsync().Result;
            var loginResult = c.Apis.Authentication.LoginAsync("4nils", "sonne").Result;

            var resourceApi = c.Api<ResourceApi>();
            //var res = resourceApi.GetAllAsync().Result;
            var lastModified = resourceApi.GetLastModifiedAsync().Result;
            var tree = c.Api<TreeApi>().TreeAsync().Result;
            var p = tree.Data.Result.Children.First().Children.First().Children.First().Path;


            var response = c.Apis.Pinboards.GetAllAsync().Result;
            var pb = c.Apis.Pinboards.GetAsync(response.Data.Result[0].Id).Result;

            var config = c.Api<ConfigApi>().ConfigAsync().Result;
            var docs = c.Api<DocumentsApi>().GetAllAsync(new SearchQuery() { Term = "Do", Limit = 2 }).Result;
            var docs2 = c.Api<DocumentsApi>().GetAllAsync().Result;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanDetectMissingProperties()
        {
            var node = new TreeNode { Title = "Hallo" };
            Check.NotNull<TreeNode, RequiredParameterAttribute>(() => node);
        }

        [TestMethod]
        public void PermissionsLoaded()
        {
            SearchQuery q = new SearchQuery(limit: 1).FilterBy(result => result.CreatedBy, "Rainer Jordan");
            var text = TestClient.Apis.Texts.GetAllExplicitAsync(q).Result;
            Assert.AreEqual(Permission.ReadWrite, text.First().Data.Result.UserPermission);
        }

        [TestMethod]
        public void BuildingGetParams()
        {
            var obj = new { id = 1, name = "flo" };
            var str = obj.ToQueryString();
            Assert.AreEqual("id=1&name=flo", str);
            string name = null;
            obj = new { id = 1, name };
            str = obj.ToQueryString();
            Assert.AreEqual("id=1", str);
        }

        [TestMethod]
        public void CanGetPossibleGroups()
        {
            var node = TestClient.Apis.Tree.FindNodeAsync(Facade.Nodes.TarifeMitMindestUmsatzId).Result; // Tarife mit mindestumsatz
            var groups = TestClient.Apis.Texts.GetGroupsAsync(node.Branches).Result;
            Assert.AreEqual(4, groups.Data.Result.Length);
            var allGroups = TestClient.Apis.Texts.GetGroupsAsync().Result;
            Assert.AreEqual(9, allGroups.Data.Result.Length);
        }


        [TestMethod]
        public void TreeApiCanCreateNodes()
        {
            var node = TestClient.Api<TreeApi>().TreeAsync().Result.Data.Result.Children[0];
            TreeNode newNode = new TreeNode()
            {
                Title = "New Tarif Node",
                Group = TestClient.Apis.Authentication.GetCurrentUserAsync().Result.Groups.First(g => g.Id == "ae84cd824c31512c014c325bc2520619"),
                CreatedBy = TestClient.Apis.Authentication.GetCurrentUserAsync().Result
            };
            var r = TestClient.Api<TreeApi>().CreateNodeAsync(newNode, node, 0).Result;
            Assert.IsNotNull(r?.Data);
            TestClient.Api<TreeApi>().DeleteNodeAsync(r.Data.Result.Id).Wait();
        }

        [TestMethod]
        public void TreeApiCanCreateNodesOnResourceNodes()
        {
            var node = TestClient.Apis.Tree.FindNodeAsync("ae84cd824c08a8b6014c08b20378004e").Result; // Tarife mit mindestumsatz
            TreeNode newNode = new TreeNode()
            {
                Title = "New Tarif Node",
                Group = TestClient.Apis.Authentication.GetCurrentUserAsync().Result.Groups.First(g => g.Id == "ae84cd824c31512c014c325bc2520619"),
                CreatedBy = TestClient.Apis.Authentication.GetCurrentUserAsync().Result
            };
            var r = TestClient.Api<TreeApi>().CreateNodeAsync(newNode, node, 0).Result;
            Assert.IsNotNull(r?.Data);
            TestClient.Api<TreeApi>().DeleteNodeAsync(r.Data.Result.Id).Wait();
        }



        [TestMethod]
        public void ConnectionTest()
        {
            var url = new Uri(TestFacade.SabioUrl);
            var client = SabioClient.CreateAsync(url.Host + "/client/realm").Result;
            Assert.AreEqual("realm", client.Realm);
            Assert.IsFalse(client.BaseUrl.AbsoluteUri.Contains("/client/realm"));
            client = SabioClient.CreateAsync(url.Host).Result;
            client.Login(Facade.Users.Admin);
            var lastModified = client.Apis.ResourceApi.GetLastModifiedAsync().Result;
            Assert.IsNotNull(lastModified);
        }

        [TestMethod]
        public void CanFindNodes()
        {
            var tree = TestClient.Api<TreeApi>().TreeAsync().Result;

            var search = TestClient.Apis.Texts.GetAllAsync(new SearchQuery(limit: 6)).Result;
            var treeNodeIds = search.Data.Result.Select(r => r.TreeNodeId).ToArray();
            var nodesInTree = treeNodeIds.Select(s => TestClient.Api<TreeApi>().FindNodeAsync(s).Result).ToArray();
            Assert.AreEqual(treeNodeIds.Length, nodesInTree.Length);
            Assert.IsTrue(nodesInTree.All(node => node != null));
        }

        [TestMethod]
        public void CanEditText()
        {
            var newTitle = "Mindestumsatz " + Guid.NewGuid() + " | " + DateTime.Now;
            SabioResponse<TextResult> text = TestClient.Api<TextsApi>().GetAllExplicitAsync(new SearchQuery("Mindestumsatz", limit: 1)).Result.First();
            text.Data.Result.Title = newTitle;
            SabioResponse<TextResult> edited = TestClient.Api<TextsApi>().EditAsync(text.Data.Result).Result;

            var loadedText = TestClient.Api<TextsApi>().GetAllExplicitAsync(new SearchQuery("Mindestumsatz", limit: 1)).Result.First();
            Assert.AreEqual(edited.Data.Result.Title, newTitle);
            Assert.AreEqual(loadedText.Data.Result.Title, newTitle);
        }

        [TestMethod]
        public void CanCreateAndDeleteDuplicate()
        {
            SabioResponse<TextResult> text = TestClient.Api<TextsApi>().GetAllExplicitAsync(new SearchQuery("Internet by Call", limit: 1)).Result.First();
            text.Data.Result.Title = $"Copy of {text.Data.Result.Title}";
            SabioResponse<TextResult> created = TestClient.Api<TextsApi>().CreateAsync(text.Data.Result).Result;

            Assert.IsTrue(created.Success);
            Assert.IsNotNull(created.Data);
            Assert.AreNotEqual(text.Data.Result.Id, created.Data.Result.Id);
            SabioResponse<TextResult> loaded = TestClient.Api<TextsApi>().GetAsync(created.Data.Result.Id).Result;
            Assert.IsNotNull(loaded.Data);
            Assert.IsTrue(loaded.Success);
            TestClient.Api<TextsApi>().DeleteAsync(created.Data.Result.Id).Wait();

            loaded = TestClient.Api<TextsApi>().GetAsync(created.Data.Result.Id).Result;
            Assert.IsNull(loaded.Data);
            Assert.IsFalse(loaded.Success);
        }


        [TestMethod]
        public void CanReadPinboard()
        {
            var res = TestClient.Apis.Pinboards.GetAllExplicitAsync().Result;
            Assert.IsTrue(!string.IsNullOrEmpty(res.First().Data.Result.Title));
        }

        [TestMethod]
        public void CanCreatePinboard()
        {
            User user = TestClient.Apis.Authentication.GetCurrentUserAsync().Result;
            var board = new Pinboard()
            {
                Content = "Moinsen",
                Title = "New Super pinnboarder " + Guid.NewGuid(),
                Group = user.Groups.First(g => g.Name == "Leiter Redaktion"),
                CreatedBy = user
            };
            var res = TestClient.Apis.Pinboards.CreateAsync(board).Result;
            Assert.IsTrue(res.Success);
            Assert.IsNotNull(res.Data);
            Assert.IsTrue(!string.IsNullOrEmpty(res.Data.Result.Title));
            TestClient.Apis.Pinboards.DeleteAsync(res.Data.Result.Id).Wait();
            var loaded = TestClient.Apis.Pinboards.GetAllAsync().Result.Data.Result.FirstOrDefault(r => r.Id == res.Data.Result.Id);
            Assert.IsNull(loaded);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanNotSendPostWhenRequiredParamsAreMissing()
        {
            var title = "New Created " + Guid.NewGuid() + " | " + DateTime.Now;

            Text textToCreate = new Text
            {
                Title = title,
            };

            TestClient.Apis.Texts.CreateAsync(textToCreate).Wait();
        }

        [TestMethod]
        public void CanCreateAndDeleteNewText()
        {
            var title = "New Created " + Guid.NewGuid() + " | " + DateTime.Now;
            var tree = TestClient.Api<TreeApi>().TreeAsync().Result;

            var nodes = new[]
            {
                tree.Data.Result.Children.First()
            };
            User user = TestClient.Apis.Authentication.GetCurrentUserAsync().Result;
            var branches = nodes.GetUniqueBranches().ToArray();
            var group = TestClient.Apis.Texts.GetGroupsAsync(branches).Result.Data.Result.Last();
            Text textToCreate = new Text
            {
                Title = title,
                Paths = nodes.ToPathsArray(),
                Branches = branches,
                Fragments = new[]
                {
                    new Fragment {
                        Content = "Sample Text",
                        Branches = branches
                    }
                },
                CreatedBy = user,
                Group = group

            };

            var created = TestClient.Apis.Texts.CreateAsync(textToCreate).Result;
            Task.Delay(3000).Wait(); // Wait because BE needs to update index
            Assert.IsNotNull(created.Data);
            Assert.IsTrue(created.Success);
            SabioResponse<TextResult> found = TestClient.Api<TextsApi>().GetAllExplicitAsync(new SearchQuery(title, limit: 1)).Result.First();
            Assert.IsNotNull(found.Data);
            Assert.IsTrue(found.Success);

            SabioResponse<TextResult> loaded = TestClient.Api<TextsApi>().GetAsync(created.Data.Result.Id).Result;
            Assert.IsNotNull(loaded.Data);
            Assert.IsTrue(loaded.Success);
            TestClient.Api<TextsApi>().DeleteAsync(created.Data.Result.Id).Wait();

            var newloaded = TestClient.Api<TextsApi>().GetAsync(created.Data.Result.Id).Result;
            Assert.IsNull(newloaded.Data);
            Assert.IsFalse(newloaded.Success);

        }

        [TestMethod]
        public void ConfigCacheTest()
        {
            var config = TestClient.Apis.Config.ConfigAsync().Result;
            var config2 = TestClient.Api<ConfigApi>().ConfigAsync().Result;
            var config3 = TestClient.Api<ConfigApi>().ConfigAsync(true).Result;

            Assert.IsTrue(ReferenceEquals(config, config2));
            Assert.IsFalse(ReferenceEquals(config, config3));
        }

        [TestMethod]
        public void CacheCanBeCleared()
        {
            var client = TestClient;
            try
            {
                client.EnableAutomaticCaching();
                client.Apis.Texts.GetAllExplicitAsync(new SearchQuery(limit: -1)).Result.Select(r => r.Data.Result);
                Assert.IsTrue(client.Cache.Count() > 0);
                client.Cache.Clear();
                Thread.Sleep(5000);
                Assert.IsTrue(client.Cache.Count() == 0, client.Cache.Count().ToString());
                client.Apis.Texts.GetAllAsync().Wait();
                var count = client.Cache.Count();
                Assert.IsTrue(count > 0);
                client.Apis.Texts.GetAllAsync().Wait();
                Assert.AreEqual(count, client.Cache.Count());
                client.Cache.Clear();
                Thread.Sleep(5000);
                Assert.IsTrue(client.Cache.Count() == 0);

            }
            finally
            {
                client.DisableAutomaticCaching();
            }
        }


        [TestMethod]
        public void CacheCanWork()
        {
            var client = TestClient;
            try
            {
                client.EnableAutomaticCaching();

                var watch = Stopwatch.StartNew();
                var allTexts = client.Apis.Texts.GetAllExplicitAsync(new SearchQuery(limit: -1)).Result.Select(r => r.Data.Result);
                watch.Stop();
                var timeLoaded = watch.Elapsed;
                Assert.IsTrue(client.Cache.Count() > 0);
                watch.Restart();
                var allTextsCached = client.Apis.Texts.GetAllExplicitAsync(new SearchQuery(limit: -1)).Result.Select(r => r.Data.Result);
                watch.Stop();
                var timeCached = watch.Elapsed;
                Assert.IsTrue(timeLoaded > timeCached * 3);
            }
            finally
            {
                client.DisableAutomaticCaching();
            }
        }

        [TestMethod]
        public void CacheDeletesAutomaticallyWhenRequired()
        {
            bool clearIsCalled = false;
            var client = TestClient;
            TextResult createdText = null;
            try
            {
                client.EnableAutomaticCaching();
                client.Cache.ClearCheckInterval = TimeSpan.FromSeconds(1);
                var allTexts = client.Apis.Texts.GetAllExplicitAsync(new SearchQuery(limit: -1)).Result.Select(r => r.Data.Result);
                Assert.IsTrue(client.Cache.Count() > 0);
                client.Apis.Texts.GetAllExplicitAsync(new SearchQuery(limit: -1)).Result.Select(r => r.Data.Result);

                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                client.Cache.Cleared += (sender, args) => clearIsCalled = true;
                createdText = CreateTextResource("Test text" + Guid.NewGuid(), "Super mega test content " + DateTime.Now);

                Thread.Sleep(5000);
                var last = client.Apis.ResourceApi.GetLastModifiedAsync().Result;
                Assert.AreEqual(createdText.Result.Id, last.Id);
                Assert.IsTrue(last.LastModifiedDate > client.Cache.LastWriteTime);
                // Trigger cache check
                client.Apis.Texts.GetAllAsync(new SearchQuery(limit: -1)).Wait();
                Thread.Sleep(5000);
                Assert.IsTrue(clearIsCalled);
                client.Apis.Texts.GetAllAsync(new SearchQuery(limit: -1)).Wait();
                Assert.IsTrue(true);
                Assert.IsTrue(client.Cache.Count() > 0);
            }
            finally
            {
                if (createdText != null)
                    client.Apis.Texts.DeleteAsync(createdText.Result.Id).Wait();
                client.DisableAutomaticCaching();
            }
        }

        [TestMethod]
        public void CanGetLastModified()
        {
            var lasModified = TestClient.Apis.ResourceApi.GetLastModifiedAsync().Result;
            Assert.IsNotNull(lasModified);
            Assert.IsNotNull(lasModified.Id);
        }

        [TestMethod]
        public void CanLoadDocuments()
        {
            var docs = TestClient.Api<DocumentsApi>().GetAllAsync(new SearchQuery() { Term = "*", Limit = 2 }).Result;
            Assert.AreEqual(2, docs.Data.Result.Length);
            var docs2 = TestClient.Api<DocumentsApi>().GetAllAsync().Result;
            var d = TestClient.Api<DocumentsApi>().GetAsync(docs2.Data.Result.First().Id).Result;
            Assert.IsNotNull(d.Data.Result.InlineUri);
            var url = TestClient.Apis.Authentication.AuthorizeUrl(d.Data.Result.InlineUri);
            Assert.IsTrue(url.Contains(TestClient.Apis.Authentication.AuthToken));
            var b = TestClient.HttpClient.GetByteArrayAsync(d.Data.Result.InlineUri).Result;
            Assert.IsNotNull(b);
            Assert.IsTrue(b.Length > 100);
        }

        [TestMethod]
        public void CanGetConfig()
        {
            var c = TestClient;
            var clientConfig = c.Api<ConfigApi>().ClientConfigAsync().Result;
            Assert.IsNotNull(clientConfig);
            var pathData = clientConfig.Data.Result.Path.Data;
            Assert.AreEqual("/sabio-web/services", pathData);

            var config = c.Api<ConfigApi>().ConfigAsync().Result;

            Assert.IsNotNull(config);
            Assert.AreEqual("4nils", config.Data.User.Login);
        }

        [TestMethod]
        public void CanGetExplicit()
        {
            var resourceApi = TestClient.Api<ResourceApi>();
            //var res = resourceApi.GetAllAsync().Result;
            var lastModified = resourceApi.GetLastModifiedAsync().Result;

            var searchQuery = new SearchQuery(limit: -1);
            var allTexts = TestClient.Apis.Texts.GetAllAsync(searchQuery).Result.Data.Result;
            var allExplicit = TestClient.Apis.Texts.GetAllExplicitAsync(searchQuery).Result.Select(r => r.Data.Result);
            Assert.AreEqual(allTexts.Length, allExplicit.Count());
        }

        [TestMethod]
        public void CanLogin()
        {
            var c = SabioClient.CreateAsync(TestFacade.SabioUrl).Result;
            var loginResult = c.Apis.Authentication.LoginAsync("4nils", "sonne").Result;
            Assert.IsTrue(loginResult.Success && !string.IsNullOrEmpty(loginResult.Data.Key));

        }

        [TestMethod]
        public void InsideCanWork()
        {
            var result = SabioClient.CreateAsync("https://inside.sabio.de/sabio/services", "inside").Result;
            var canWork = result.Api<ConfigApi>().ClientConfigAsync().Result.Success;
            Assert.IsTrue(canWork);
            Assert.IsTrue(result.CanWorkAsync().Result);
        }

    }
}
