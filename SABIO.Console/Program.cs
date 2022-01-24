using System;
using System.Linq;
using SABIO.ClientApi.Core;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Types;

namespace SABIO.Console
{
    class Program
    {

        static void Main(string[] args)
        {
            var c = new SabioClient("https://pcfg.test3.sabio.de/sabio-web/services", "qa-test");
            c.Apis.Authentication.LoginAsync("4nils", "sonne").Wait();
            
            var config = c.Api<ConfigApi>().ConfigAsync().Result;
            var docs = c.Api<DocumentsApi>().GetAllAsync(new SearchQuery() { Term = "Do", Limit = 2 }).Result;
            var docs2 = c.Api<DocumentsApi>().GetAllAsync().Result;
            var d = c.Api<DocumentsApi>().GetAsync(docs2.Data.Result.First().Id).Result;
            var b = c.HttpClient.GetByteArrayAsync(d.Data.Result.InlineUri).Result;


            var texts = c.Api<TextsApi>().GetAllAsync().Result;
            var text = c.Api<TextsApi>().GetAsync(texts.Data.Result.First().Id);
            System.Console.ReadLine();
        }
    }
}
