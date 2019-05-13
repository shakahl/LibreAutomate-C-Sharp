# Command line

These program files have command line parameters:
- Au.Editor.exe - the editor program.
- Au.CL.exe - the "run script" program.

#### Au.Editor.exe

| Parameter | Description |
| - | - |
| `"file or folder path"` | Import it into the current workspace. The program shows a dialog.<br/>Can be multiple files, like `"file1" "file2" "file3"`. |
| `"workspace folder path"` | Open or import the workspace. The program shows a dialog. |
| `/n` or `-n` | Run not as administrator. See [UAC](../articles/UAC.md). When started without this as not administrator, the program restarts as administrator, unless not installed correctly. When started as administrator, the program runs as administrator. |

#### Au.CL.exe

This small and fast program is used to execute scripts. It relays the script name and parameters to the editor process, which compiles and starts the script.

The command line is the script name and optionally script's command line parameters (variable *args*), separated by space and optionally enclosed in "". The script must exist in current workspace.

Use prefix * to wait until the script ends.

Use prefix ** to wait until the script ends and capture its [ATask.WriteResult]() text. To capture **Console.Write** text, need to compile the script to .exe and run the .exe file instead.

The exit code of this program when it waits is the script's exit code. To set it the script can use **Environment.ExitCode**. When does not wait, the exit code normally is 0. When fails to run (script not found, contains errors, etc), the exit code is a negative value.

##### Examples

`Au.CL.exe Script5.cs`
`Au.CL.exe "Script name with spaces.cs"`
`Au.CL.exe Script5.cs /a "some text"`
`Au.CL.exe *Script5.cs`
`Au.CL.exe **Script5.cs`

The .cs is optional.
