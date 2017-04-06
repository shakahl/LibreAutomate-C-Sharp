ReadProcessMemory (triggers Avira FP),WriteProcessMemory (currently unused),VirtualAllocEx,VirtualFreeEx (currently unused). These used in CProcessMemory, currently only to get .NET control name.
Process32NextW (triggers Avira FP, don't know why), Process32FirstW CreateToolhelp32Snapshot
GetModuleFileName
FindNextFile etc
OpenProcess

SetWindowsHookExW, UnhookWindowsHookEx, CallNextHookEx
SetWinEventHook, UnhookWinEvent
AllowSetForegroundWindow
AttachThreadInput
SetClipboardViewer
SendInput
PrintWindow

AdjustTokenPrivileges
Registry functions

ShellExecuteExW

WTSEnumerateProcessesW

WS2_32.DLL and all its functions.

 These used in qm but not in exe
CreateProcessW etc
ReadDirectoryChangesW

 These not used, but remember in future
CreateRemoteThread
DebugActiveProcess


 Some of these functions trigger FP eg in Avira.
 Increasing code size (eg 340 to 350 KB) may fix the FP. Also no FP in qm.exe, which contains all these functions. Probably small files containing big number of "dangerous" API look suspicious to AV.
