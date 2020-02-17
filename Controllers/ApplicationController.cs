using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace metawatcher.Controllers
{
  public class ApplicationController
  {
    private Services.ApiService apiService = new Services.ApiService(ContentType);
    private Services.RoutesService routes = new Services.RoutesService();

    // protected
    protected const string ContentType = "application/json";

    protected Services.ApiService ApiService 
    { get => apiService; }

    protected Services.RoutesService Routes 
    { get => routes; }

    protected dynamic StrongParams
    { get; set; }

    protected HttpResponseMessage Response
    { get => ApiService.HttpResponseMessage; }

    protected dynamic DeserializeContent(dynamic content, dynamic structure)
    {
      return JsonConvert.DeserializeAnonymousType(content, structure);
    }

    protected StringContent SerializeContent(dynamic content)
    {
      return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8);    
    }
  }
}