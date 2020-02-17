using System.Drawing;
using System.Diagnostics;

namespace metawatcher.Services
{
  public class ActiveProcessService
  {
    private Process activeProcess;
    private User32.Rect rectActiveProcess = new User32.Rect();

    public ActiveProcessService(Process process)
    {
      ActiveProcess = process;
    }

    public Process ActiveProcess
    {
      get => activeProcess;
      set 
      {
        User32.GetWindowRect(value.MainWindowHandle, ref rectActiveProcess);
        activeProcess = value;
      }
    }

    public int Height
    { get => rectActiveProcess.Right - rectActiveProcess.Left; }

    public int Width
    { get => rectActiveProcess.Bottom - rectActiveProcess.Top; }

    public Size Size
    { get => new Size(Width, Height); }

    public int Top
    { get => rectActiveProcess.Top; }

    public int Left
    { get => rectActiveProcess.Left; }

    public int Id
    { get => activeProcess.Id; }
  }
}