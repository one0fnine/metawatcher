using System.Collections.Generic;
using System.Threading.Tasks;

namespace metawatcher.Controllers
{
  public class ProfileController : ApplicationController
  {
    public async Task Update(List<string> content)
    {
      ApiService.ContentType = ContentType;
      StrongParams = SerializeContent(new { signed_id = content });
      await ApiService.Call(Routes.Profile, StrongParams);
    }
  }
}