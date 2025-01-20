using Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCitiesAPI.Tests
{
  public static class HttpClientExtensions
  {

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="endpoint"></param>
    /// <param name="serializedData"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    public static async Task<T> InvokePostAsync<T>(
      this HttpClient client,
      string endpoint,
      string serializedData, 
      string mediaType)
    {
      /*
       * Short version for ---> 
       * var loginContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
       * var loginResponse = await _client.PostAsync("/api/Account/Login", loginContent);
       * loginResponse.EnsureSuccessStatusCode();
       * JsonConvert.DeserializeObject<LoginResult>(await loginResponse.Content.ReadAsStringAsync());
       */
      StringContent httpContent = null!;
      HttpResponseMessage? httpResponse = null!;
      T? objResponse;

      httpContent = new StringContent(serializedData, Encoding.UTF8, mediaType);
      httpResponse = await client.PostAsync(endpoint, httpContent);

      if (httpResponse.IsSuccessStatusCode)
      {
        objResponse = JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());
      }
      else
      {
        objResponse = default(T);
      }

      #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
      #pragma warning disable CS8603 // Possible null reference return.
      return (T)(objResponse ?? Activator.CreateInstance(typeof(T)));
      #pragma warning restore CS8603 // Possible null reference return.
      #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }



  }
}
