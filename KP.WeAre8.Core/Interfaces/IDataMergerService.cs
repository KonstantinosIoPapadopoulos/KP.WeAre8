using KP.WeAre8.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KP.WeAre8.Core.Interfaces
{
    public interface IDataMergerService
    {
        public Task<Result<List<Dictionary<string, object>>>> FetchDataAsync(string urlAddress);
        public Task<Result<List<Dictionary<string, object>>>> ReturnDataAsync(string urlAddress1, string urlAddress2);
        public Result<List<Dictionary<string, object>>> SortDescDataAsync(List<Dictionary<string, object>> listOfDictionaries);

        // General Use
        public Task<Result<List<Dictionary<string, object>>>> ReturnDataAsync(string[] ArrayOfUrlAddresses, SortByRule[] sortByData);
        public Result<List<Dictionary<string, object>>> SortDataAsync(List<Dictionary<string, object>> listOfDictionaries, SortByRule[] sortByRules);
    }
}
