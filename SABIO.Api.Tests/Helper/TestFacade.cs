using System;

namespace SABIO.Api.Tests.Helper
{
    public class TestFacade
    {
        public UserFacade Users = new();
        public FilesFacade Files = new();
        public string SabioUrl = "https://maestro-fg-knowledge.labs.swops.cloud/sabio-web/services";
        public string Realm = "qa-test";
        
    }
}