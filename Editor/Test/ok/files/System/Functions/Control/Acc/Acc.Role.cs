function# [str&rolestr] [str&rolestr2]

 Gets role.
 Returns numeric <google "site:microsoft.com object role constants IAccessible ROLE_SYSTEM_GRIP">role</google> value.

 rolestr - str variable that receives role string as it is used in QM. It is role constant name without "ROLE_SYSTEM_" prefix. For nonstandard roles it is like rolestr2.
 rolestr2 - str variable that receives role string retrieved using <google>GetRoleText</google>.

 REMARKS
 In some programs some roles are strings. For such roles returns 0. Then use rolestr or rolestr2 to get role string.
 If fails to get role, returns 0 and stores "unknown" in rolestr and rolestr2. Usually it happens for objects that already don't exist.

 EXAMPLES
 if(a.Role=ROLE_SYSTEM_PUSHBUTTON) out "button"

 a.Role(_s); out _s


if(!a) end ERR_INIT
VARIANT v=a.Role(elem); err
sel v.vt
	case VT_BSTR
	if(&rolestr) rolestr=v
	if(&rolestr2) rolestr2=v
	
	case else
	int r=v; err
	if(r)
		BSTR b
		if(&rolestr)
			if(r<=ROLE_SYSTEM_OUTLINEBUTTON) rolestr.gett("TITLEBAR,MENUBAR,SCROLLBAR,GRIP,SOUND,CURSOR,CARET,ALERT,WINDOW,CLIENT,MENUPOPUP,MENUITEM,TOOLTIP,APPLICATION,DOCUMENT,PANE,CHART,DIALOG,BORDER,GROUPING,SEPARATOR,TOOLBAR,STATUSBAR,TABLE,COLUMNHEADER,ROWHEADER,COLUMN,ROW,CELL,LINK,HELPBALLOON,CHARACTER,LIST,LISTITEM,OUTLINE,OUTLINEITEM,PAGETAB,PROPERTYPAGE,INDICATOR,GRAPHIC,STATICTEXT,TEXT,PUSHBUTTON,CHECKBUTTON,RADIOBUTTON,COMBOBOX,DROPLIST,PROGRESSBAR,DIAL,HOTKEYFIELD,SLIDER,SPINBUTTON,DIAGRAM,ANIMATION,EQUATION,BUTTONDROPDOWN,BUTTONMENU,BUTTONDROPDOWNGRID,WHITESPACE,PAGETABLIST,CLOCK,SPLITBUTTON,IPADDRESS,OUTLINEBUTTON" r-1)
			else GetRoleTextW(r b.alloc(100) 100); rolestr.ansi(b)
		if(&rolestr2)
			GetRoleTextW(r b.alloc(100) 100); rolestr2.ansi(b)
		ret r
	else
		if(&rolestr) rolestr="unknown"
		if(&rolestr2) rolestr2="unknown"
