using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace metawatcher
{
  class Program
  {
    private const string ProcessName = "terminal", AltTabKeys = "%+{TAB}"; 
    private static Services.ProcessService processService = new Services.ProcessService(ProcessName);
    private static Services.ScreenshotService screenshotService = new Services.ScreenshotService();
    private static Controllers.ScreenshotsController screenshotController = new Controllers.ScreenshotsController();

    static async Task Main()
    {
      List<string> signedIds = new List<string>();
      Hashtable selectedProcesses = processService.SelectedProcesses();

      try {
        while(selectedProcesses.Count > 0)
        {
          var activeProcess = processService.ActiveProcess();

          if(selectedProcesses.Contains(activeProcess.Id))
          {
            selectedProcesses.Remove(activeProcess.Id); 

            screenshotService.Width = activeProcess.Width;
            screenshotService.Height = activeProcess.Height;
            screenshotService.Size = activeProcess.Size;
            screenshotService.Left = activeProcess.Left;
            screenshotService.Top = activeProcess.Top;

            using(var memoryStream = screenshotService.ScreenshotStream())
            {
              await screenshotController.Create(memoryStream);
              signedIds.Add(screenshotController.DeserializedResponse.signed_id);
            }
          }

          SendKeys.SendWait(AltTabKeys);
          Thread.Sleep(500);
        }

        await new Controllers.ProfileController().Update(signedIds);
      } catch {
        Console.WriteLine("Something went wrong");
      }
    }
  }
}
