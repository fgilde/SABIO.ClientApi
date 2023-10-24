using SABIO.ClientApi.Core;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.Api.Tests.Helper.Facades;

public class TreeNodeFacade: FacadeBase
{
    public string TarifeMitMindestUmsatzId = "ae84cd824c08a8b6014c08b20378004e";
    
    public TreeNode TarifeMitMindestUmsatz => Client.Apis.Tree.FindNodeAsync(TarifeMitMindestUmsatzId).Result;

    public TreeNodeFacade(SabioClient client) : base(client)
    {}
}