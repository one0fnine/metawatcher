using System;
using System.Net.Http;
using System.Collections;
namespace metawatcher.Services
{
  public class RoutesService
  {
    private const string ApiHost = "https://be94d2a5.ngrok.io";
    private const string ApiVersion = "/api/v1";
    private Hashtable routes = new Hashtable();

    public RoutesService()
    {
      routes.Add("direct_upload", new { Uri = new Uri($"{ApiHost}{ApiVersion}/direct_uploads"), Method = HttpMethod.Post });
      routes.Add("profile", new { Uri = new Uri($"{ApiHost}{ApiVersion}/profile"), Method = HttpMethod.Put });
    }

    public dynamic DirectUpload
    { get => routes["direct_upload"]; }

    public dynamic Profile
    { get => routes["profile"]; }
  }
}