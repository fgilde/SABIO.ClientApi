using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SABIO.ClientApi.Core.Api;

namespace SABIO.Api.Tests;

[TestClass]
public class TreeApiTests : TestBase<TreeApi>
{
    [TestMethod]
    public void CanCreateNodeStructure()
    {
        var parentNode = Facade.Nodes.TarifeMitMindestUmsatz;
        var result = Api.CreateNodeStructureAsync("/test/subtest/hello", parentNode).Result;
        Assert.AreEqual(3, result.Count);
        parentNode = Api.FindNodeAsync(Facade.Nodes.TarifeMitMindestUmsatzId).Result;
        Assert.IsTrue(parentNode.Children.Any(n => n.Id == result.First().Id));
        Api.DeleteNodeAsync(result.First().Id);
        parentNode = Api.FindNodeAsync(Facade.Nodes.TarifeMitMindestUmsatzId).Result;
        Assert.IsFalse(parentNode.Children.Any(n => n.Id == result.First().Id));
    }

    [TestMethod]
    public void CanCreateNodeStructureAndSkipExisting()
    {
        var nodesTarifeMitMindestUmsatz = Facade.Nodes.TarifeMitMindestUmsatz;
        var parentNode = nodesTarifeMitMindestUmsatz.ParentNode;
        var result = Api.CreateNodeStructureAsync($"/{nodesTarifeMitMindestUmsatz.Title}/test/subtest/hello", parentNode).Result;
        Assert.AreEqual(4, result.Count);
        Assert.AreEqual(3, result.Count(n => n.NewlyCreated));
        var toDelete = result.First(n => n.NewlyCreated);

        parentNode = Api.FindNodeAsync(Facade.Nodes.TarifeMitMindestUmsatzId).Result.ParentNode; // reload to ensure children filled

        result = Api.CreateNodeStructureAsync($"/{nodesTarifeMitMindestUmsatz.Title}/test/subtest/hello", parentNode).Result;
        Assert.AreEqual(4, result.Count);
        Assert.AreEqual(0, result.Count(n => n.NewlyCreated));

        result = Api.CreateNodeStructureAsync($"/{nodesTarifeMitMindestUmsatz.Title}/test/subtest/undersub/underundersub", parentNode).Result;
        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(2, result.Count(n => n.NewlyCreated));


        parentNode = Api.FindNodeAsync(Facade.Nodes.TarifeMitMindestUmsatzId).Result;
        Assert.IsTrue(parentNode.Children.Any(n => n.Id == toDelete.Id));

        Api.DeleteNodeAsync(toDelete.Id);
        parentNode = Api.FindNodeAsync(Facade.Nodes.TarifeMitMindestUmsatzId).Result;
        Assert.IsFalse(parentNode.Children.Any(n => n.Id == toDelete.Id));
    }

}