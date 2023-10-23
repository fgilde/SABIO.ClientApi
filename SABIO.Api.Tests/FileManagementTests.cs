using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Responses.Types;
using System.Linq;
using System.Threading.Tasks;
using SABIO.Api.Tests.Helper;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Types;
using File = SABIO.ClientApi.Responses.Types.File;

namespace SABIO.Api.Tests;

[TestClass]
public class FileManagementTests : TestBase<FileManagementApi>
{
    [TestMethod]
    public void FileManagementIsEnabledOnTestSystem()
    {
        var config = TestClient.Apis.Config.ConfigAsync().Result;
        Assert.IsTrue(config.Data.System.FileManagementEnabled);
    }

    [TestMethod]
    public void CanCreateWordFileAndAttachItToNewText()
    {
        File file = new File
        {
            Title = "Test File",
            Filename = FilesFacade.WildRiceRecipes,
            Owner = TestClient.Apis.Authentication.GetCurrentUserAsync().Result,
            OwnerGroup = TestClient.Apis.Texts.GetGroupsAsync().Result.Data.Result.First(),
            TargetGroups = TestClient.Apis.Texts.GetGroupsAsync().Result.Data.Result
        };

        var response = Api.CreateFileAsync(file.ToUploadableFile(Facade.Files.WildRiceRecipesData)).Result;
        Assert.IsTrue(response.Success);

        var createdFile = response.Data.Result;


        var title = "New Created for " + file.Title;
        var tree = TestClient.Api<TreeApi>().TreeAsync().Result;

        var nodes = new[]
        {
            tree.Data.Result.Children.First()
        };
        User user = TestClient.Apis.Authentication.GetCurrentUserAsync().Result;
        var branches = nodes.GetUniqueBranches().ToArray();
        Text textToCreate = new Text
        {
            Title = title,
            Paths = nodes.ToPathsArray(),
            Branches = branches,
            Fragments = new[]
            {
                new Fragment {
                    Content = "Sample with attached file",
                    Branches = branches,
                    AttachedFiles = new[] { createdFile.Id }
                }
            },
            CreatedBy = user,
            Group = TestClient.Apis.Texts.GetGroupsAsync(branches).Result.Data.Result.First()
            //Group = user.Groups.First(g => g.Name == "Atlantis GER Agent") // Das sabio BE ist so kacke, dass man bei dieser Gruppe z.B gar keinen Response mehr bekommt, der server also irgendwo direkt austeigt

        };

        var created = TestClient.Apis.Texts.CreateAsync(textToCreate).Result;
        Task.Delay(3000).Wait(); // Wait because BE needs to update index
        Assert.IsNotNull(created.Data);
        Assert.IsTrue(created.Success);
        SabioResponse<TextResult> found = TestClient.Api<TextsApi>().GetAllExplicitAsync(new SearchQuery(title, limit: 1)).Result.First();
       

    }

    [TestMethod]
    public void CanCreateFolderStructure()
    {
        var result = Api.CreateFolderStructureAsync("/test/subtest/hello").Result;
        var secondResult = Api.CreateFolderStructureAsync("/test/subtest/MoiNsen").Result;
        Assert.AreEqual(3, result.Count);
        var liveStructure = Api.GetAllAsync().Result.Data.Result;
        Assert.IsTrue(liveStructure.Any(f => f.Id == result.First().Id));
        Api.DeleteFolderAsync(result.First());
        liveStructure = Api.GetAllAsync().Result.Data.Result;
        Assert.IsFalse(liveStructure.Any(f => f.Id == result.First().Id));
    }

    [TestMethod]
    public void CanCreateFolder()
    {
        var result = Api.CreateFolderAsync(new FileFolder { Title = "Test Folder" }).Result;
        Assert.IsTrue(result.Success);
        var createdFolder = result.Data.Result.First();
        var createdExists = Api.GetAllAsync().Result.Data.Result.Any(f => f.Id == createdFolder.Id);
        Assert.IsTrue(createdExists);

        Api.DeleteFolderAsync(createdFolder);
        createdExists = Api.GetAllAsync().Result.Data.Result.Any(f => f.Id == createdFolder.Id);
        Assert.IsFalse(createdExists);
    }

    [TestMethod]
    public void CanCreateAndDeleteWordFile()
    {
        File file = new File
        {
            Title = "Test File",
            Filename = FilesFacade.WildRiceRecipes,
            Owner = TestClient.Apis.Authentication.GetCurrentUserAsync().Result,
            OwnerGroup = TestClient.Apis.Texts.GetGroupsAsync().Result.Data.Result.First(),
            TargetGroups = TestClient.Apis.Texts.GetGroupsAsync().Result.Data.Result
        };

        var response = Api.CreateFileAsync(file.ToUploadableFile(Facade.Files.WildRiceRecipesData)).Result;
        Assert.IsTrue(response.Success);

        var createdFile = response.Data.Result;
        var createdExists = Api.GetAllAsync().Result.Data.Result.Any(f => f.Id == createdFile.Id);
        Assert.IsTrue(createdExists);

        Api.DeleteFileAsync(createdFile);
        createdExists = Api.GetAllAsync().Result.Data.Result.Any(f => f.Id == createdFile.Id);
        Assert.IsFalse(createdExists);
    }

    [TestMethod]
    public void CanCreateAndDeleteTextFile()
    {
        File file = new File
        {
            Title = "Test File",
            Filename = FilesFacade.TestTxt,
            Owner = TestClient.Apis.Authentication.GetCurrentUserAsync().Result,
            OwnerGroup = TestClient.Apis.Texts.GetGroupsAsync().Result.Data.Result.First(),
            TargetGroups = TestClient.Apis.Texts.GetGroupsAsync().Result.Data.Result
        };

        var response = Api.CreateFileAsync(file.ToUploadableFile(Facade.Files.TestTxtData)).Result;
        Assert.IsTrue(response.Success);

        var createdFile = response.Data.Result;
        var createdExists = Api.GetAllAsync().Result.Data.Result.Any(f => f.Id == createdFile.Id);
        Assert.IsTrue(createdExists);

        Api.DeleteFileAsync(createdFile);
        createdExists = Api.GetAllAsync().Result.Data.Result.Any(f => f.Id == createdFile.Id);
        Assert.IsFalse(createdExists);
    }

    [TestMethod]
    public void CanRead()
    {
        var fm = TestClient.Api<FileManagementApi>();
        var all = fm.GetAllAsync().Result;
        Assert.IsTrue(all.Success);
    }
}