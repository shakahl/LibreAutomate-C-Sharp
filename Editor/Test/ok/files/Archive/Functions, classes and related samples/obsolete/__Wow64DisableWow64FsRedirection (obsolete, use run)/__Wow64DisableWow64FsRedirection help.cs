 On Windows 64-bit, there are 2 versions of system folder: system32 and SysWOW64.
 system32 contain 64-bit files. It is used by 64-bit applications.
 SysWOW64 contain 32-bit versions of these files. It is used by 32-bit applications, including QM.
 If a function in QM or other 32-bit application tries to access a file in system32 folder, Windows redirects it to the SysWOW64 folder. Normally 32-bit applications in function calls use system32, not SysWOW64; then the same code works on all Windows version.
 This behavior is the same with QM file functions (run, str.searchpath, etc) and Windows API functions.
 The 32-bit folder contains 32-bit versions of many of the system files. However not all.
 Use this class to temporarily disable this redirection, eg when you want to run a 64-bit system file.
 Call DisableRedirection when need redirection. Call Revert as soon as possible when already don't need disabling, because other functions may fail when redirection disabled.
 The class reverts automatically when the variable dies if Revert not called.
 Disables only in current thread.
 With run() use flag 0x30000, or may not work.
 The class does nothing on 32-bit Windows. Not error.
 Note: not all system folder subfolders/files are redirected. Some redirected differently.
 More info: google for "File System Redirector" or Wow64DisableWow64FsRedirection.

 You probably noticed that on Windows 64-bit also there are 2 program files folders. The "Program Files" folder contains 64-bit apps; the "Program Files (x86)" folder contains 32-bit apps.
 The redirection is not applied to program files folders. Don't need this class.
 In QM, special folder "$program files$" expands to the 32-bit folder ("Program Files (x86)"). QM is installed there. Environment variable "%ProgramW6432%" expands to the 64-bit folder ("Program Files").

 EXAMPLE

#compile "____Wow64DisableWow64FsRedirection"
__Wow64DisableWow64FsRedirection x.DisableRedirection
run "c:\windows\system32\msconfig.exe" "" "" "" 0x30000 ;;msconfig.exe exe is unavailable in the 32-bit folder, therefore run would fail if redirection not disabled
x.Revert
