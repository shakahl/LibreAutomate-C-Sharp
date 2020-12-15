---
uid: command_line
---

# Command line

These program files have command line parameters:
- Aedit.exe - the automation script editor program.
- Au.CL.exe - the "run script using command line" program.

#### Aedit.exe

| Parameter | Description |
| - | - |
| /v or -v | Show the main window when started.<br/>When started without this, the window may be invisible, it depends on program settings; to show it, click the tray icon or run the program twice. |
| `/n` or `-n` | Run not as administrator. See [UAC](xref:uac).<br/>When started without this as not administrator, the program restarts as administrator, unless not installed correctly. When started as administrator, the program runs as administrator. |
| `"file or folder path"` | Import it into the current workspace. The program shows a dialog.<br/>Can be multiple files, like `"file1" "file2" "file3"`. |
| `"workspace folder path"` | Open or import the workspace. The program shows a dialog. |

#### Au.CL.exe

This small and fast program is used to execute scripts like command line programs. It relays the script name and parameters to the editor process, which compiles and starts the script.

The command line is the script name and optionally command line arguments, separated by space and optionally enclosed in "". Arguments will be in the *args* variable of the script's main function. The script must exist in current workspace.

Use prefix * to wait until the script ends. Use prefix ** to wait until the script ends and capture its [ATask.WriteResult]() text. To capture **Console.Write** text, instead compile the script to .exe and run the .exe file.

The exit code of this program when it waits is the script's exit code. To set it the script can use **Environment.ExitCode**. When does not wait, the exit code is the process id of the script. When fails to run (script not found, contains errors, etc), the exit code is < 0.

This program starts the editor process if not running. To start editor and don't execute a script, use command line `/e`. It is slightly faster way to start editor process as administrator.

##### Examples

- `Au.CL.exe Script5.cs`
- `Au.CL.exe "Script name with spaces.cs"`
- `Au.CL.exe Script5.cs /example "argument with spaces"`
- `Au.CL.exe *Script5.cs`
- `Au.CL.exe **Script5.cs`
- `Au.CL.exe /e`

The .cs is optional.
