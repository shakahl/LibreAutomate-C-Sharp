def FILE_NOTIFY_CHANGE_FILE_NAME 0x1
def FILE_NOTIFY_CHANGE_DIR_NAME 0x2
def FILE_NOTIFY_CHANGE_ATTRIBUTES 0x4
def FILE_NOTIFY_CHANGE_SIZE 0x8
def FILE_NOTIFY_CHANGE_LAST_WRITE 0x10
def FILE_NOTIFY_CHANGE_LAST_ACCESS 0x20
def FILE_NOTIFY_CHANGE_CREATION 0x40
def FILE_NOTIFY_CHANGE_SECURITY 0x100

dll kernel32
	#FindFirstChangeNotification $lpPathName bWatchSubtree dwNotifyFilter
	#FindNextChangeNotification hChangeHandle
	#FindCloseChangeNotification hChangeHandle

lpstr sFolder
sFolder="$desktop$"

str s.expandpath(sFolder)

int h=FindFirstChangeNotification(s 0 FILE_NOTIFY_CHANGE_LAST_WRITE)
if(h=-1) end ES_FAILED
wait 0 H h
FindCloseChangeNotification h
mes "yes"

