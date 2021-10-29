using KP.WeAre8.Core.Interfaces;
using KP.WeAre8.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KP.WeAre8.Core.Services
{
    class DataMergerService : IDataMergerService
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use.
        static readonly HttpClient client = new HttpClient();

        public async Task<Result<List<Dictionary<string, object>>>> FetchDataAsync(string urlAddress)
        {
            if (urlAddress == null)
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.Unspecified, "The List are NULL");
            }
            string responseBody = string.Empty;
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlAddress);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.NotFound, "Requested URL is not valid");
            }
            dynamic responseData = JsonConvert.DeserializeObject(responseBody, typeof(object));
            var returnData = new List<Dictionary<string, object>>();        //Convert It to List<Dictionary<string, object>>
            foreach (JObject obj in responseData)
            {
                Dictionary<string, object> myDictionary = new Dictionary<string, object>();
                foreach (JProperty prop in obj.Properties())
                {
                    myDictionary.Add(prop.Name, prop.Value);
                }
                returnData.Add(myDictionary);
            }
            return new Result<List<Dictionary<string, object>>>
            {
                Data = returnData
            };
        }

        public async Task<Result<List<Dictionary<string, object>>>> ReturnDataAsync(string urlAddress1, string urlAddress2)
        {
            if ((urlAddress1 == null) || (urlAddress2 == null))
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.Unspecified, "The List are NULL");
            }
            var listOfDictionaries1 = await FetchDataAsync(urlAddress1);
            var listOfDictionaries2 = await FetchDataAsync(urlAddress2);
            if ((listOfDictionaries1.Error is not null) || (listOfDictionaries2.Error is not null))
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.NotFound, "Requested URL is not valid");
            }
            // Merge the Dictionaries
            try
            {
                listOfDictionaries1.Data.AddRange(listOfDictionaries2.Data);
            }
            catch (Exception)
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.Conflict, "The two List can Not be combinded");
            }
            // Sort the list Of Dictionaries
            var sortedList = SortDescDataAsync(listOfDictionaries1.Data);
            if (sortedList.Data == null)
            {
                return new Result<List<Dictionary<string, object>>>(sortedList.Error.ErrorCode, sortedList.Error.Message); ;
            }
            return new Result<List<Dictionary<string, object>>>
            {
                Data = sortedList.Data
            };
        }
        public Result<List<Dictionary<string, object>>> SortDescDataAsync(List<Dictionary<string, object>> listOfDictionaries)
        {
            if (listOfDictionaries == null)
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.Unspecified, "The List are NULL");
            }
            var sortedList = new List<Dictionary<string, object>>();
            try
            {
                sortedList = listOfDictionaries.OrderByDescending(itemDictionary => itemDictionary["updatedDate"]).ToList();
            }
            catch (Exception)
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.Unspecified, "The List can Not be Sorted");
            }
            return new Result<List<Dictionary<string, object>>>
            {
                Data = sortedList
            };
        }

        public async Task<Result<List<Dictionary<string, object>>>> ReturnDataAsync(string[] ArrayOfUrlAddresses, SortByRule[] sortByData)
        {
            if ((ArrayOfUrlAddresses == null) || (sortByData == null))
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.Unspecified, "The List are NULL");
            }
            var listOfDictionaries = new List<Dictionary<string, object>>();
            foreach (string UrlAddress in ArrayOfUrlAddresses)
            {
                var Dictionary = await FetchDataAsync(UrlAddress);
                if (Dictionary.Data == null)    // Error in Url  
                {
                    return new Result<List<Dictionary<string, object>>>(ErrorCode.NotFound, "Requested URL is not valid");
                }
                try     //// Merge the Dictionaries
                {
                    listOfDictionaries.AddRange(Dictionary.Data);
                }
                catch (Exception)
                {
                    return new Result<List<Dictionary<string, object>>>(ErrorCode.Conflict, "The two List can Not be combinded");
                }
            }
            // Sort the list Of Dictionaries
            var sortedList = SortDataAsync(listOfDictionaries, sortByData);
            if (sortedList.Data == null)
            {
                return new Result<List<Dictionary<string, object>>>(sortedList.Error.ErrorCode, sortedList.Error.Message); ;
            }
            return new Result<List<Dictionary<string, object>>>
            {
                Data = sortedList.Data
            };
        }

        public Result<List<Dictionary<string, object>>> SortDataAsync(List<Dictionary<string, object>> listOfDictionaries, SortByRule[] sortByRules)
        {
            if ((listOfDictionaries == null) || (sortByRules == null)){
                return new Result<List<Dictionary<string, object>>>(ErrorCode.Unspecified, "The List are NULL");
            }
            if (sortByRules.Length < 1)  // Empty -> No Order
            {
                return new Result<List<Dictionary<string, object>>> { Data = listOfDictionaries };
            }
            // fasle -> ASC || true -> DESC 
            try
            {
                var sortedList = sortByRules[0].SortOrderDesc ?
                    listOfDictionaries.OrderByDescending(itemDictionary => itemDictionary[sortByRules[0].SortKey]) :
                    listOfDictionaries.OrderBy(itemDictionary => itemDictionary[sortByRules[0].SortKey]);
                for (int i = 1; i < sortByRules.Length; i++)
                {
                    sortedList = sortByRules[i].SortOrderDesc ?
                        sortedList.ThenByDescending(itemDictionary => itemDictionary[sortByRules[0].SortKey]) :
                        sortedList.ThenBy(itemDictionary => itemDictionary[sortByRules[0].SortKey]);
                }
                return new Result<List<Dictionary<string, object>>>
                {
                    Data = sortedList.ToList()
                };
            }
            catch (Exception)
            {
                return new Result<List<Dictionary<string, object>>>(ErrorCode.Unspecified, "The List can Not be Sorted");
            }
        }
    }
}
