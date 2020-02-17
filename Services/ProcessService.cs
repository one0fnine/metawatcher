using System.Collections;
using System.Linq;
using System.Diagnostics;

namespace metawatcher.Services
{
  public class ProcessService
  {
    private string processName;
    private Hashtable selectedProcesses = new Hashtable();

    public ProcessService(string name) 
    {
      this.processName = name;
    }

    public int Count
    { get => CurrentProcessesByName.Count(); }

    public ActiveProcessService ActiveProcess()
    {
      uint processID = 0;
      System.IntPtr hWnd = User32.GetForegroundWindow();
      uint threadID = User32.GetWindowThreadProcessId(hWnd, out processID);
      
      return new ActiveProcessService(Process.GetProcessById(System.Convert.ToInt32(processID)));
    }

    public Hashtable SelectedProcesses()
    {
      selectedProcesses.Clear();
      for(int i = 0; i < CurrentProcessesByName.Length; i++)
      { selectedProcesses.Add(CurrentProcessesByName[i].Id, CurrentProcessesByName[i]); };

      return selectedProcesses;
    }

    private Process[] CurrentProcessesByName
    { get => Process.GetProcessesByName(processName); }
  }
}