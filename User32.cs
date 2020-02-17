using System.Runtime.InteropServices;

namespace metawatcher
{
  public class User32
  {
    [DllImport("user32.dll")]
    public static extern System.IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(System.IntPtr hWnd, out uint lpdwProcessId); 

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(System.IntPtr hWnd, ref Rect lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }   
  }
}