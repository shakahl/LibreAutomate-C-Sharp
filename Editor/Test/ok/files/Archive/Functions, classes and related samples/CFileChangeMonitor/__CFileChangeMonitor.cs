class CFileChangeMonitor -m_snid

 need to use ordinal on win2000
dll shell32
	[2]#SHChangeNotifyRegister hwnd fSources fEvents wMsg cEntries SHChangeNotifyEntry*pshcne
	[4]#SHChangeNotifyDeregister ulID

 this is undefined and undocumented
type SHNOTIFYSTRUCT dwItem1 dwItem2 ;;dwItem1 contains the PIDL or name of the folder. dwItem2 contains the new PIDL or name of the folder when renamed. 
