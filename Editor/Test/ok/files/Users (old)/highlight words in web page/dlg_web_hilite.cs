\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=
 <A href="javascript:function H(w,s){r=w.document.body.createTextRange();for(i=0;r.findText(s);i++){r.execCommand('BackColor','','yellow');r.collapse(false);}return i;}function G(){if(frames.length==0)return document.selection.createRange().text;else for(k=0;F=frames[k];++k){u=F.document.selection.createRange().text;if(u)return u;}}function P(){var t=0,s=G();if(!s)s=prompt('Find:','');if(s){if(frames.length==0)t+=H(window,s);else for(j=0;F=frames[j];++j)t+=H(F,s);alert(t+' found.');}}P();" ADD_DATE="1044141169" LAST_MODIFIED="0" LAST_VISIT="0">Hilite Text</A>
 <p>one two three</p>
 <p>one two three</p>
if(!ShowDialog("dlg_web_hilite" &dlg_web_hilite &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 465 346 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 468 346 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "*" "" ""

ret
 messages
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
