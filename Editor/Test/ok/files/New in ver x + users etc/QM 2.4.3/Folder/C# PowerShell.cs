int R=CsExec("")
out R

#ret
//C# code
namespace HostSamples
{
  using System;
  using System.Management.Automation;  // Windows PowerShell namespace.

  /// <summary>
  /// This class defines the main entry point for a host application that
  /// synchronously invokes the following pipeline:
  /// [Get-Process]
  /// </summary>
  internal class HostPS1
  {
    /// <summary>
    /// The PowerShell object is created and manipulated within the
    /// Main method.
    /// </summary>
    /// <param name="args">This parameter is not used.</param>
    private static void Main(string[] args)
    {
      // Call the PowerShell.Create() method to create an
      // empty pipeline.
      PowerShell ps = PowerShell.Create();

      // Call the PowerShell.AddCommand(string) method to add
      // the Get-Process cmdlet to the pipeline. Do
      // not include spaces before or after the cmdlet name
      // because that will cause the command to fail.
      ps.AddCommand("Get-Process");

      Console.WriteLine("Process                 Id");
      Console.WriteLine("----------------------------");

      // Call the PowerShell.Invoke() method to run the
      // commands of the pipeline.
        foreach (PSObject result in ps.Invoke())
      {
        Console.WriteLine(
                "{0,-24}{1}",
                result.Members["ProcessName"].Value,
                result.Members["Id"].Value);
      } // End foreach.
    } // End Main.
  } // End HostPs1.
}
