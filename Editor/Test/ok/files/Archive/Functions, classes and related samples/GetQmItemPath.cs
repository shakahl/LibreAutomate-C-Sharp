 /
function VARIANT'item str&s ;;item can be name or id

 Gets QM item path.

 item - item name or id (see qmitem in help).
 s - receives full path.

 EXAMPLE
 str s
 GetQmItemPath "GetCurDir" s
 out s

out item.vt
int iid
sel item.vt
	case [VT_I4,VT_I2] iid=item.iVal
	case VT_BSTR iid=qmitem(s.from(item.bstrVal))
if(!iid) end ES_BADARG

s.all
QMITEM qi
rep
	qmitem iid 0 qi 17
	s.from("\" qi.name s)
	iid=qi.folderid
	if(!iid) break
	