 /Compare QM folder with old folder
function# iid QMITEM&q level str&s

lpstr k="changed"

 get old macro
if(q.itype=7) ret ;;file link
int fl; if(q.itype=5) fl=1; lpstr sFold=" (folder)" ;;folder
str sOld.GetMacroFromFile("" q.name fl s)
err k=_error.description; goto g1
if(sOld=q.text) ret
if(!sOld.len and !q.text.len) ret ;;one may be "", other null
 g1
out F"<><open>{q.name}</open>{sFold}: {k}"
