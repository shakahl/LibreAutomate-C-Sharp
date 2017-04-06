int process_handle
process_handle=run("notepad.exe")
5
TerminateProcess process_handle 1
CloseHandle process_handle
