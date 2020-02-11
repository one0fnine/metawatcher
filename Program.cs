using System.Collections;
using System.Linq;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace metawatcher
{
  class User32
  {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId); 

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }   
  }

  class Program
  {
    public const string ProcessName = "terminal", 
      AltTabKeys = "%+{TAB}", ScreenshotContentType = "image/png", 
      ScreenshotExtension = "png", ApiUrl = "https://aade867c.ngrok.io", ContentType = "application/json",
      ApiDirectUploadPath = "/meta/screens/direct_uploads";
    
    static async Task Main()
    {
      Process[] currentTerminals = Process.GetProcessesByName(ProcessName);
      var processedTerminals = new Hashtable();
      int terminalCounter = 0;
      var httpClient = new HttpClient();

      while(terminalCounter < currentTerminals.Count())
      {
        // get an active process
        // TODO: move into User32 class
        uint processID = 0;
        IntPtr hWnd = User32.GetForegroundWindow();
        uint threadID = User32.GetWindowThreadProcessId(hWnd, out processID);
        var activeProcess = Process.GetProcessById(Convert.ToInt32(processID));

        // check the active process
        if(activeProcess.ProcessName == ProcessName)
        {
          try 
          { 
            processedTerminals.Add(activeProcess.Id, activeProcess); 
            // take the screenshot of the active process
            // get WxH of the process
            var processRect = new User32.Rect();
            User32.GetWindowRect(activeProcess.MainWindowHandle, ref processRect);
            int width = processRect.Right - processRect.Left;
            int height = processRect.Bottom - processRect.Top;
            using(var image = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
              using(var graphics = Graphics.FromImage(image))
              {
                graphics.CopyFromScreen(processRect.Left, processRect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                //save to memory
                using(var memoryStream = new MemoryStream())
                {
                  image.Save(memoryStream, ImageFormat.Png);
                  // prepare params
                  using(var md5Hash = MD5.Create())
                  {
                    byte[] md5Bytes = md5Hash.ComputeHash(memoryStream.ToArray());
                    string fileName = activeProcess.MainWindowTitle.Split(':').First();
                    dynamic strongParams = new
                    {
                      blob = new {
                        filename = $"{fileName}.{ScreenshotExtension}",
                        content_type = ScreenshotContentType,
                        checksum = Convert.ToBase64String(md5Bytes),
                        byte_size = memoryStream.Length
                      }
                    };
                    var json = JsonConvert.SerializeObject(strongParams);
                    var content = new StringContent(json, Encoding.UTF8, ContentType);

                    //send data
                    var response = await httpClient.PostAsync(ApiUrl + ApiDirectUploadPath, content);
                    response.EnsureSuccessStatusCode();
                    var responseJson = await response.Content.ReadAsStringAsync();
                    // prepare response
                    var directUploadsResponse = new
                    {
                      signed_id = "",
                      direct_upload = new { url = "" }
                    };
                    var directUploadsResponseObject = JsonConvert.DeserializeAnonymousType(responseJson, directUploadsResponse);
                    // send a screenshot
                    var httpRequestMessage = new HttpRequestMessage();
                    httpRequestMessage.RequestUri = new Uri(directUploadsResponseObject.direct_upload.url);
                    httpRequestMessage.Method = HttpMethod.Put;
                    httpRequestMessage.Content = new ByteArrayContent(memoryStream.ToArray());
                    httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(ScreenshotContentType);
                    response = await httpClient.SendAsync(httpRequestMessage);
                    response.EnsureSuccessStatusCode();
                    // update an account
                    var usedCPU = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    var freeRAM = new PerformanceCounter("Memory", "Available MBytes");
                    usedCPU.NextValue();
                    freeRAM.NextValue();
                    await Task.Delay(500);
                    strongParams = new 
                    { 
                      signed_id = directUploadsResponseObject.signed_id,
                      used_cpu = usedCPU.NextValue(),
                      free_memory = freeRAM.NextValue()
                    };
                    json = JsonConvert.SerializeObject(strongParams);

                    httpRequestMessage = new HttpRequestMessage();
                    httpRequestMessage.Method = HttpMethod.Put;
                    httpRequestMessage.RequestUri = new Uri(ApiUrl + "/api/v1/profile");
                    httpRequestMessage.Content = new StringContent(json, Encoding.UTF8, ContentType);
                    response = await httpClient.SendAsync(httpRequestMessage);
                    response.EnsureSuccessStatusCode();
                  }
                }
              }
            }
            terminalCounter += 1;
          } 
          catch { break; }
        }

        SendKeys.SendWait(AltTabKeys);
        Thread.Sleep(1000);
      }
    }
  }
}
