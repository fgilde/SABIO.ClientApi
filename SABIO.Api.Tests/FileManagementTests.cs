using Microsoft.VisualStudio.TestTools.UnitTesting;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Responses.Types;
using System.Linq;
using SABIO.Api.Tests.Helper;
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
            Title = "Test File", Filename = FilesFacade.TestTxt,
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