 /
function# $md [int&haccel] [flags] ;;flags: 1 popup menu

 Takes text that contains menu definition and creates a menu bar menu or a popup menu (flag 1).
 Returns menu handle. Returns 0 if failed (eg menu definition not found).

 md - menu definition.
   If md is single line, it is interpreted as name of QM item (macro etc) that contains menu definition.
   If md is : followed by a number (eg ":100"), instead loads menu from resource whose id is that number. This can be used in exe.
 haccel - variable that receives a handle of accelerator table. This function creates it from the menu definition.
   If using resource, loads accelerator table with the same id as the menu (if exists).
   Later call DestroyAcceleratorTable, unless you pass it to DT_SetMenu.
   If haccel not used, does not create/load accelerators.
 flags (QM 2.4.2):
   1 - create a popup menu, not a menu bar menu. Read more in remarks. Must not be a menu resource.

 REMARKS
 You can create menu definitions with the Menu Editor. Look in floating toolbar -> More Tools.
 Read more in <help #IDH_DIALOG_EDITOR#A13>Help</help>.

 This function creates a standard menu. Any Windows API menu functions can be used to manipulate it.
 By default creates a menu that can be added to a dialog or other window as a menu bar. Pass the returned handle to <help>DT_SetMenu</help> or SetMenu. Usually you don't call this function, instead pass menu definition to ShowDialog.
 If flag 1 used, creates a popup menu instead. To show it, pass the returned handle to TrackPopupMenuEx. Or instead use ShowMenu or MenuPopup.Create/Show.
 Later call DestroyMenu, unless you pass the handle to DT_SetMenu or SetMenu or assign to a MenuPopup variable.


#region
#opt nowarnings 1

if(empty(md)) ret
if md[0]=':'
	int hmenu idmenu hres
	hres=GetExeResHandle; if(!hres) end F"cannot create menu. Failed to load resource {md}" 8; ret
	idmenu=val(md+1)
	hmenu=LoadMenu(hres +idmenu)
	if(&haccel) haccel=LoadAccelerators(hres +idmenu)
	ret hmenu
else if(findc(md 10)<0) md=_s.getmacro(md); err end F"cannot create menu. {_error.description}" 8; ret
#endregion

str s
if(findrx(md __S_RX_MD 0 0 s 1)<0) end F"cannot create menu. Menu definition not found" 8; ret
ARRAY(lpstr) a
if(!tok(s a -1 "[]" 1)) ret

ARRAY(ACCEL) aac
int hm=iif(flags&1 CreatePopupMenu CreateMenu)
sub.CreateSubmenu(hm a 0 iif(&haccel aac 0))
if(&haccel) if(aac.len) haccel=CreateAcceleratorTable(&aac[0] aac.len); else haccel=0
ret hm


#sub CreateSubmenu
function# hm ARRAY(lpstr)&a i ARRAY(ACCEL)&aac

MENUITEMINFOW mi
int vert nt vk mod
lpstr s sId sType sState sHK
str st

for i i a.len
	memset &mi 0 sizeof(mi); mi.cbSize=sizeof(mi); mi.fMask=MIIM_TYPE|MIIM_ID
	
	s=a[i]; s+strspn(s " [9],;")
	sel s[0]
		case '-': mi.fType=MFT_SEPARATOR; goto gIns
		case '|': vert=MFT_MENUBARBREAK; continue
		case '>':
			mi.hSubMenu=CreatePopupMenu
			i=sub.CreateSubmenu(mi.hSubMenu a i+1 aac)
			s+1; mi.fMask|MIIM_SUBMENU
		case '<': break
	
	vk=0
	sId=strstr(s " :")
	if sId
		sId[0]=0
		nt=tok(sId+2 &sId 4 " [9]''" 5)
		if(nt) mi.wID=val(sId)
		if(nt>1) mi.fType=val(sType)
		if(nt>2) mi.fState=val(sState); mi.fMask|MIIM_STATE
		if(nt>3 and sHK[0] and TO_HotkeyFromQmKeys(sHK mod vk) and &aac) ACCEL ac.key=vk; ac.fVirt=mod<<2|1; ac.cmd=mi.wID; aac[]=ac
	
	st=s; st.escape
	if(vk and findc(st 9)<0) st+"[9]"; FormatKeyString vk mod &st
	mi.dwTypeData=@st
	
	 gIns
	mi.fType|vert; vert=0
	InsertMenuItemW(hm 0xffff 1 &mi)

ret i
