 /
function hwnd `tab

 Selects a tab in tab control.

 hwnd - control handle.
 tab - 0-based tab index.
   QM 2.4.1. Also can be tab name. Full or wildcard. Error if tab not found.

 REMARKS
 Works only with standard tab controls. The class often is "SysTabControl32", but not always.
 For other tab controls try <help>Acc.Select</help>(3) or <help>Acc.DoDefaultAction</help>, see example.

 EXAMPLES
 int hwnd=id(12320 "Display Prop")
 SelectTab hwnd 5

  Acc.Select example
 int w1=win("Options" "#32770")
 Acc a.Find(w1 "PAGETAB" "Files" "class=SysTabControl32" 0x1005)
 a.Select(3) ;;if this does not work, try a.DoDefaultAction


if(!hwnd) end ERR_HWND

int i
sel tab.vt
	case VT_BSTR
	opt noerrorshere 1
	_s=tab.bstrVal
	Acc a=acc(_s "PAGETAB" hwnd "" "" 0x1001)
	i=a.elem-1
	
	case else
	i=tab.lVal

SendMessage(hwnd TCM_SETCURFOCUS i 0)
