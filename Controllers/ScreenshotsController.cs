using System.Text;
using System.Net.Http;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;

namespace metawatcher.Controllers
{
  public class ScreenshotsController : ApplicationController
  {
    public async Task Create(MemoryStream content)
    {
      await CreateBlob(content);
      await UploadSreenshot(content);
    }

    public dynamic DeserializedResponse
    { get; private set; }

    // private
    private dynamic responseStructure = new { signed_id = "", content_type = "", direct_upload = new { url = "" } };

    private async Task UploadSreenshot(MemoryStream stream)
    {
      var response = await Response.Content.ReadAsStringAsync();
      DeserializedResponse = DeserializeContent(response, responseStructure);
      var strongParams = new ByteArrayContent(stream.ToArray());
      ApiService.ContentType = DeserializedResponse.content_type;
      var route = new { Uri = new Uri(DeserializedResponse.direct_upload.url), Method = HttpMethod.Put };
      await ApiService.Call(route, strongParams);
    }

    private async Task CreateBlob(MemoryStream stream)
    {
      PrepareDirectUploadParams(stream);
      ApiService.ContentType = ContentType;
      await ApiService.Call(Routes.DirectUpload, StrongParams);
    }

    private void PrepareDirectUploadParams(MemoryStream stream)
    {
      using(var md5Hash = MD5.Create())
      {
        byte[] md5Bytes = md5Hash.ComputeHash(stream.ToArray());
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < md5Bytes.Length; i++)  
        { builder.Append(md5Bytes[i].ToString("x2")); }          

        StrongParams = SerializeContent(new {
          blob = new {
            filename = $"{builder.ToString()}.png",
            content_type = "image/png",
            checksum = Convert.ToBase64String(md5Bytes),
            byte_size = stream.Length
          }
        });
      }
    }
  }
}