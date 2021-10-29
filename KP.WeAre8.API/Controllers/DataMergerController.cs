using KP.WeAre8.Core.Interfaces;
using KP.WeAre8.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace KP.WeAre8.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataMergerController : Controller
    {
        private readonly IDataMergerService _dataMergerService;

        public DataMergerController(IDataMergerService dataMergerService)
        {
            _dataMergerService = dataMergerService;
        }

        // GET: /DataMerger
        [HttpGet]
        public async Task<string> Get()
        {
            string urlAddress1 = "https://cdn-test.test.aws.the8app.com/feedsponsorships.json";
            string urlAddress2 = "https://cdn-test.test.aws.the8app.com/communitytiles.json";

            var results = await _dataMergerService.ReturnDataAsync(urlAddress1, urlAddress2);
            string returnData = JsonConvert.SerializeObject(results.Data);
            return returnData;
        }

        // POST /DataMerger
        [HttpPost]
        public async Task<string> Post(PostData postData)
        {
            //  General Example
            //string[] Urls = {"https://cdn-test.test.aws.the8app.com/feedsponsorships.json",
            //    "https://cdn-test.test.aws.the8app.com/communitytiles.json",
            //    "https://cdn-test.test.aws.the8app.com/communitytiles.json" };
            //SortByRule[] rules = { new SortByRule() { SortKey = "updatedDate", SortOrderDesc  = true },

            //new SortByRule() { SortKey = "updatedDate", SortOrderDesc  = false }};
            //var results = await _dataMergerService.ReturnDataAsync(Urls, rules);

            /*
            {
                "urls": [
                  "https://cdn-test.test.aws.the8app.com/feedsponsorships.json",
                "https://cdn-test.test.aws.the8app.com/communitytiles.json",
                "https://cdn-test.test.aws.the8app.com/communitytiles.json"
              ],
              "rules": [
                {
                    "sortKey": "updatedDate",
                  "sortOrderDesc": true
                },
                {
                    "sortKey": "createdDate",
                  "sortOrderDesc": false
                }
              ]
            }
            */

            var results = await _dataMergerService.ReturnDataAsync(postData.Urls, postData.Rules);
            return JsonConvert.SerializeObject(results.Data);
        }
    }
}
