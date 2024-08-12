using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendWatch.App.DataModels;

namespace TrendWatch.App.Helpers
{
    public static class ApiService
    {
        public static async Task<string> Generate(string prmpt)
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/Gen?p="+ prmpt);
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);
                    string result = "";
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ApiResponse<string>>(content);
                        if (responseResult.success)
                        {
                            result = responseResult.data;
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }
    }
}
