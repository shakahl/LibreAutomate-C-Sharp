int+ hjhook
hjhook=SetWindowsHookEx(WH_JOURNALRECORD &JournalProc _hinst 0)
out hjhook ;;must be in dll
if(!hjhook) ret
atend UnhookWindowsHookEx hjhook
mes ""
