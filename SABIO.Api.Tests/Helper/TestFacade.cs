using System;
using SABIO.Api.Tests.Helper.Facades;
using SABIO.ClientApi.Core;

namespace SABIO.Api.Tests.Helper
{
    public class TestFacade : FacadeBase
    {
        public const string SabioUrl = "https://maestro-fg-knowledge.labs.swops.cloud";

        public UserFacade Users;
        public FilesFacade Files;
        public TreeNodeFacade Nodes;
        
        public TestFacade(SabioClient client) : base(client)
        {
            Users = new(client);
            Files = new(client);
            Nodes = new(client);
        }
    }
}