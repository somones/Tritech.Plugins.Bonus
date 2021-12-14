using MetaQuotes.MT5CommonAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tritech.Plugins.Test.Log4Net;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;



namespace Tritech.Plugins.Test
{
     [TestClass]
     public class Mt5ApiTest
     {

          HttpClient client;

          public Mt5ApiTest()
          {
               client = new HttpClient();
          }


          [TestMethod]
          public async Task Mt5GetuserTest()
          {               
               HttpResponseMessage response = await client.GetAsync("");
               response.EnsureSuccessStatusCode();
               string data = await response.Content.ReadAsStringAsync();
               bool result = data.Contains(@"""Login"":");
               Logger.Log($"{{ result : {result} ,  data : {data} }}");
               Assert.IsTrue(result, "Get user failed");
          }

     }
}
