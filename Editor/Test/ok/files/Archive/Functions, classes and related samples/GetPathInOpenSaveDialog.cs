 /
function hwnd [str&sFolder] [str&sFilename]

 Gets folder path and/or filename from an Open or Save As dialog box.
 Error if fails.

 hwnd - handle of the dialog box.
 sFolder - variable that receives current folder.
 sFilename - variable that receives filename. Note that it is raw text of the File name edit box and therefore can be anything.

 REMARKS
 Tested on Windows 7, 8 and XP. May not work on future Windows versions. On XP fails with some dialogs (depends on style).

 EXAMPLE
 int w=win("Open" "#32770")
 str sFolder sFilename
 GetPathInOpenSaveDialog w sFolder sFilename
 out F"{sFolder}\{sFilename}"


if(!IsWindow(hwnd)) end ERR_WINDOW
if _winnt>=6
	Acc a; int c
	if &sFolder
		c=child("" "Breadcrumb Parent" hwnd)
		if(c) c=child("" "ToolbarWindow32" c)
		if(!c) end ERR_FAILED
		a.Find(c "TOOLBAR" "" "" 0x1005)
		_s=a.Name
		int i=find(_s ": "); if(i<0) end ERR_FAILED ;;"Address: ..."
		sFolder.get(_s i+2)
	if &sFilename
		c=child("" "Edit" hwnd 0x400 "id=1148") ;;Open
		if(!c) c=child("" "ComboBox" hwnd 0x400 "cclass=Edit") ;;Save As
		a.Find(c "TEXT" "" "" 0x1005)
		sFilename=a.Value
else
	__ProcessMemory m.Alloc(hwnd 1000)
	int n
	if &sFolder
		n=SendMessageW(hwnd CDM_GETFOLDERPATH 500 m.address); if(n<1) end ERR_FAILED
		m.ReadStr(sFolder n*2 0 1)
	if &sFilename
		n=SendMessageW(hwnd CDM_GETSPEC 500 m.address); if(n<1) end ERR_FAILED
		m.ReadStr(sFilename n*2 0 1)

err+ end _error
