using System.Text;
using System.Text.Json;
using QuartzNetWebConsole.Utils;

namespace OtherWay.Radio.Scheduler.Services;

public class OtherwayAPIProxy {
  private readonly IHttpClientFactory _httpClientFactory;

  public OtherwayAPIProxy(IHttpClientFactory httpClientFactory) {
    _httpClientFactory = httpClientFactory;
  }

  private HttpClient _getClient() => _httpClientFactory.CreateClient("otherway");

  public async Task<string> MakePost(string endpoint, object payload) {
    try {
      using var client = _getClient();

      var response = await client.PostAsync(endpoint,
        new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
      return response.IsSuccessStatusCode ? string.Empty : response.ReasonPhrase ?? "Unknown error calling API";
    }
    catch (Exception e) {
      return e.Message;
    }
  }

  public async Task<string?> MakeGet(string endpoint) {
    try {
      using var client = _getClient();
      var response = await client.GetAsync(endpoint);

      return response.IsSuccessStatusCode ? string.Empty : response.ReasonPhrase ?? "Unknown error calling API";
    }
    catch (Exception e) {
      return e.Message;
    }
  }
}