 /
function# [$ddMacro] [dlgProc] [!*controls] [hwndOwner] [flags] [style] [notStyle] [param] [x] [y] [$icon] [$menu] ;;flags: 1 modeless, 4 set style (default - add), 64 raw x y, 128 hidden, 0x100 DD x y

 Creates and shows dialog box.
 Returns: 1 OK, 0 Cancel. If modeless, returns window handle.

 <help #IDH_DIALOG_EDITOR>Dialog help</help>.

 ddMacro - name of macro containing dialog definition (DD). Can be empty if DD is in the caller. Or DD string (if multiline).
   Dialog definition is text that begins with BEGIN DIALOG and ends with END DIALOG, created with the Dialog Editor.
 dlgProc - for smart dialogs - address of <help #IDH_DIALOG_EDITOR#A9>dialog procedure</help>. Default: 0 - simple dialog.
 controls - address of <help #IDH_DIALOG_EDITOR#A5>dialog variables</help>. Default: 0 - don't set/get control values.
 hwndOwner - owner window handle.
 flags - see above.
   Flag 128 added in QM 2.3.3. Flag 0x100 added in QM 2.4.2.
 style - dialog window styles (WS_...) to add or set.
 notStyle - dialog window styles to remove.
 param - some value that dialog procedure can access with DT_GetParam.
 x, y - dialog <help #IDH_DIALOG_EDITOR#A3>coordinates</help>. Depends on hwndOwner, _monitor variable, flag 64, and these styles: DS_CENTERMOUSE, DS_CENTER, DS_ABSALIGN, WS_CHILD.
 icon - title bar icon.
   Supports <help #IDP_RESOURCES>macro resources</help> (QM 2.4.1) and exe resources.
 menu - <help #IDH_DIALOG_EDITOR#A13>menu</help>. Can be name of macro that contains menu definition, or menu definition string, or menu resource id like ":1".


#opt nowarnings 1
opt nowarningshere 1

str txt s controlsErr
int iid i r isOwner cbFunc cbParam
lpstr es=".[][9]First argument of ShowDialog must be name of macro or function that contains dialog definition (text that begins with BEGIN DIALOG). Or it can be variable that contains dialog definition."

 get dialog definition
if(flags&2 or findc(ddMacro 10)>=0) txt=ddMacro
else
	iid=iif(empty(ddMacro) getopt(itemid 1) qmitem(ddMacro 5))
	if(!iid) end F"{ERR_BADARG}. Macro not found or is encrypted{es}"
	txt.getmacro(iid)

if(dlgProc and !IsValidCallback(dlgProc 16)) end F"{ERR_BADARG} (dlgProc)"

__DIALOG d
if(flags&0x80000000) cbFunc=x; cbParam=y; x=0; y=0
if(!cbFunc) cbFunc=&sub.CompileCallback; cbParam=&d; int validateIds=1 ;;if there are tooltips in dialog definition, creates tt and adds to it

 convert
i=sub_DT.CompileDialog(txt s cbFunc cbParam)
if(!i) end F"{ERR_BADARG}. Dialog definition not found or is invalid{es}"

 validate controls and get specified ids to d.acid
if(!sub_DT.ParseControlsVar(d +controls d.acid controlsErr validateIds)) end F"{ERR_BADARG} (controls).{controlsErr}"

 style
DLGTEMPLATEEX* dt=s
int& dst=dt.style
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
d.controls=controls
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
if(!empty(menu)) d.hmenu=DT_CreateMenu(menu d.haccel)

 needs memory allocated by GlobalAlloc
__GlobalMem dtm=GlobalAlloc(0 i)
memcpy(+dtm s i); s.all

 create dialog window
if(flags&0x81) ;;modeless
	r=DialogBoxIndirectParamU(1 +dtm hwndOwner &DT_DialogProc &d)
	if(!r) r=-1
	else if(flags&1=0) r=sub.Modal(r) ;;hidden. Must be modeless, but behave almost like modal.
else
	if(isOwner and GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0; d.flags2|0x200 ;;prevent disabling; DT_Init will set owner
	r=DialogBoxIndirectParamU(0 +dtm hwndOwner &DT_DialogProc &d)

if(r=-1) end F"{ERR_FAILED}. Cannot show dialog. The most common reason is an invalid value in dialog definition: an unknown control class or an invalid control style"

if(flags&1=0) WaitForAnActiveWindow 500 2 ;;if modal, process messages, or later something will not work, eg GetKeyState
ret r

err+ end F"Error (RT) in dialog: {_error.description},[][9]line: {_error.line}" 8

 note: HWND_MESSAGE cannot be used with modal


#sub CompileCallback
function DLGTEMPLATEEX*dt DLGITEMTEMPLATEEX*dit $cls $txt __DIALOG&d $tooltip


if dit
	d.acid[]=dit.id
	if !empty(tooltip)
		if(!d.tt) d.tt._new
		d.tt.AddControl(dit.id tooltip)
else if !empty(tooltip)
	lpstr s1 s2
	tok tooltip &s1 2 " "
	d.ttFlags=val(s1); d.ttTime=val(s2)


#sub Modal
function# hDlg

int mel=RegisterWindowMessage("WM_QM_ENDDIALOG")

MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	if(m.message=mel) ret m.wParam
	if(IsDialogMessage(hDlg &m)) continue
	TranslateMessage &m
	DispatchMessage &m

if(m.message=WM_QUIT) PostQuitMessage(m.wParam)
