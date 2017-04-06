function! idMacro $sub flags ;;flags: 0 dialog editor, 1 menu editor

 Finds dialog procedure for events.

str sErr("Please click Save, then retry.") sdm(iif(flags&1 "menu" "dialog"))

 find DD/MD sub
if(!idMacro) goto ge
Init(idMacro)
int i iSub=FindSub(sub); if(iSub<0) goto ge
__Sub& r=a[iSub]

 find dialog procedure
str dpName
e.iDP=-1
 try to get dialog procedure name from ShowDialog statement
FINDRX f.ifrom=r.codeOffset; f.ito=f.ifrom+r.codeLen
if findrx(sText "(?m)^[\t,]*(?=[^ ;/\\\t,]).*?\bShowDialog[\( ] *(?:''.*?''|[^\s,\)]+)[ ,]+&((?:sub\.)?\w+)" f 0 dpName 1)>=0
	if(dpName.beg("sub.")) e.iDP=FindSub(dpName+4); if(e.iDP<0) sErr=F"Cannot find dialog procedure {dpName} specified in ShowDialog statement."; goto ge2
	else if(dpName=_s.getmacro(idMacro 1)) e.iDP=0
	if(e.iDP>=0 and !IsDlgProc(e.iDP 1)) e.iDP=-1
else ;;find a dialog procedure in this macro
	for(i iSub a.len) if(IsDlgProc(i 1)) e.iDP=i; break
	if(e.iDP<0) for(i 0 iSub) if(IsDlgProc(i 1)) e.iDP=i; break
	if e.iDP>=0 ;;show warning, but only if there are multiple dialogs
		int ndd; for(i 0 a.len) if(a[i].dd.len) ndd+1
		if(ndd>1) out "Warning: could not find a ShowDialog statement with a dialog procedure address. The event code may be inserted in a wrong dialog procedure. The ShowDialog statement should be in the same function or sub-function where is the %s definition." sdm
if(e.iDP>=0) ret 1

sErr=F"Cannot find dialog procedure.[]It must be in the same macro/function where is the {sdm} definition."
 ge2
sErr+"[]More info in QM output."
_s=
 <>Sample ShowDialog code and dialog procedure:
 <code>
 if(!ShowDialog(dd &sub.DlgProc 0)) ret
;
;
 #sub DlgProc
 function# hDlg message wParam lParam
;
 sel message
 	case WM_INITDIALOG
 	case WM_DESTROY
 	case WM_COMMAND goto messages2
 ret
  messages2
 sel wParam
 	case IDOK
 	case IDCANCEL
 ret 1
 </code>
out _s

 ge
mes sErr
