 out
typelib VsHelp {83285928-227C-11D3-B870-00C04F79F802} 1.0
VsHelp.DExploreAppObj dex._getactive
 dex.Filter="C Run-Time Libraries (CRT)"
 dex.Filter="(no filter)"
 dex.Filter="Platform SDK"

str fn
 fn="BitBlt"
 fn="IsIconic"
 fn="WM_TIMER"
 fn="strcpy"
 fn="StrCpy"
 fn="_seterrormode"
 fn="_cgets"
 fn="EnumObjects"
fn="Escape"
 fn="LZClose"
 fn="GetPixelFormat"
 fn="memchr"
 fn="_cprintf"
str s
if(!GetFuncArgsFromDoc3(dex fn s 0)) ret
out s
1
act _hwndqm
