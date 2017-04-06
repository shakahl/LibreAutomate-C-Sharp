dll user32 #CreateIconFromResourceEx !*pbIconBits cbIconBits fIcon dwVersion cxDesired cyDesired uFlags 





int hm=LoadLibraryEx("shell32" 0 LOAD_LIBRARY_AS_DATAFILE)
 int hm=LoadLibraryEx("notepad.exe" 0 0)
 int hm=LoadLibraryEx("record.ico" 0 0)
EnumResourceNames(hm +RT_GROUP_ICON &EnumResNameProc 0)
FreeLibrary(hm)
