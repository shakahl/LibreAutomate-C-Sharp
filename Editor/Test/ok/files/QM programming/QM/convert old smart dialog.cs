str s name("") rx dd
 out
 name="Macro2520"
s.getmacro(name)
name.getmacro(name 1)

_i=findrx(s __S_RX_DDDE 0 0 dd); if(_i<0) out "dialog definition not found"; ret
s.remove(_i dd.len)

rx=
F
 (?ms)^(function# hDlg message wParam lParam.+?)^if\(hDlg\) goto messages
 (.*?)^((?:str controls\b.+?ShowDialog|\S*?ShowDialog)\()"(?:{name}|)" &{name}\b(.+?)^ret
 (.*?)^ messages
;
_i=s.replacerx(rx F"$2str dd=[]{dd}[]$3dd &sub.DlgProc$4#sub DlgProc[]$1$5" 4)
if(_i<0) out F"rx not found:[]{rx}"; ret
 out s
s.setmacro("")
