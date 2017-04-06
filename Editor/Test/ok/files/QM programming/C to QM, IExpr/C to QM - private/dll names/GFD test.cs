 out
typelib VsHelp {83285928-227C-11D3-B870-00C04F79F802} 1.0
VsHelp.DExploreAppObj dex._getactive
dex.Filter="Platform SDK"

str fn
fn="BitBlt"
 fn="IsIconic"
str s
if(!GetFuncDllFromDoc3(dex fn s)) ret
out s
1
act _hwndqm
