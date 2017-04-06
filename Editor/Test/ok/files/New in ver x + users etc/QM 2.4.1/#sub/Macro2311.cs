 /
function# [$ddMacro] [dlgProc] [!*controls] [hwndOwner] [flags] [style] [notStyle] [param] [x] [y] [$icon] [$menu] ;;flags: 1 modeless, 4 set style (default - add), 64 raw x y, 128 hidden

            kdslhd hjgf
str txt s
int iid i r isOwner cbFunc(iif(flags&0x80000000 param 0)) cbParam
lpstr es=".[][9]First argument of ShowDialog must be name of macro or function that contains dialog definition (text that begins with BEGIN DIALOG). Or it can be variable that contains dialog definition."

 get dialog definition
if(flags&2 or findc(ddMacro 10)>=0) txt=ddMacro
else
	iid=iif(empty(ddMacro) getopt(itemid 1) qmitem(ddMacro 5))
	if(!iid) end F"{ERR_BADARG}. Macro not found or is encrypted{es}"
	txt.getmacro(iid)

if(dlgProc and !IsValidCallback(dlgProc 16)) end ERR_BADARG

__DIALOG d
if(!cbFunc) cbFunc=&DT_CompileCallback; cbParam=&d ;;if there are tooltips in dialog definition, creates tt and adds to it

 convert
i=CompileDialog(txt s cbFunc cbParam)
if(!i) end F"{ERR_BADARG}. Dialog definition not found or is invalid{es}"

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal

#sub A
 style
int& dst=+(s+12)
if(flags&4) dst=style
 else if(flags&8) dst|style ;;fbc
else if(flags&16) dst~style ;;fbc
else if(flags&32) dst^style ;;fbc
else dst|style
dst~notStyle
isOwner=hwndOwner and hwndOwner!HWND_MESSAGE
if(!isOwner) dst~WS_CHILD
if(dst&WS_CHILD) flags|1
else
	if isOwner
		hwndOwner=GetAncestor(hwndOwner 2)
		if(GetWindowLong(hwndOwner GWL_EXSTYLE)&WS_EX_TOPMOST) int& dest=+(s+8); dest|WS_EX_TOPMOST
	if(flags&128) dst~WS_VISIBLE|DS_SETFOREGROUND
	if(dst&DS_SETFOREGROUND) AllowActivateWindows

 dialog data
d.dlgproc=dlgProc
d.pdata=controls
if(isOwner) d.hwndowner=hwndOwner
d.flags=flags
d.param=param
d.x=x
d.y=y
d.style=dst
if(dst&(WS_VISIBLE|DS_SETFOREGROUND)=0) d.flags2|0x100

 icon
if(!empty(icon))
	d.hicon16=GetFileIcon(icon)
	if(d.hicon16) d.hicon32=GetFileIcon(icon 0 1)
	else end F"cannot load icon {icon}" 8
else ;;use icon of thread main function (in exe - exe icon)
	d.hicon16=__GetQmItemIcon(+getopt(itemid 3))
	d.hicon32=__GetQmItemIcon(+getopt(itemid 3) 1)

 menu
if(!empty(menu))
	d.hmenu=DT_CreateMenu(menu d.haccel)
	if(!d.hmenu) end F"cannot create or load menu {menu}" 8

 needs memory allocated by GlobalAlloc
__GlobalMem dt=GlobalAlloc(0 i)
memcpy(+dt s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dt hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=DT_DoModal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dt hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal
  hhh