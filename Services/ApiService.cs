using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace metawatcher.Services
{
  public class ApiService
  {  
    public ApiService(string _contentType)
    {
      contentType = _contentType;
    }

    public HttpResponseMessage HttpResponseMessage
    { get; private set; }

    public string ContentType
    { get => contentType; set => contentType = value; }

    public async Task Call(dynamic route, dynamic strongParams)
    {
      try
      {
        Uri = route.Uri;
        HttpMethod = route.Method;
        StrongParams = strongParams;
        await SendAsync();
      }
      catch
      {
        Console.WriteLine("Something went wrong in API Service");
      }
    }

    //private
    private string contentType = "application/json";
    private HttpClient httpClient = new HttpClient();

    private HttpMethod HttpMethod
    { get; set; }

    private HttpClient HttpClient
    { get => httpClient; }

    private Uri Uri
    { get; set; }

    private dynamic StrongParams
    { get; set; }

    private async Task SendAsync()
    {
      using(var httpRequestMessage = new HttpRequestMessage())
      {
        httpRequestMessage.Method = HttpMethod;
        httpRequestMessage.RequestUri = Uri;
        httpRequestMessage.Content = StrongParams;
        httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
        HttpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);
      }
    }
  }
}