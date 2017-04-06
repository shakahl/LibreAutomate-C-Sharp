 /
function` $typeName `v

int vt
sel typeName
	case "bool" vt=VT_BOOL
	case else end ERR_BADARG

if(vt=v.vt) ret v

VARIANT r
int hr=VariantChangeTypeEx(&r &v 0x409 0 vt)
if(hr) end F"Cannot change type. {_s.dllerror(`` `` hr)}"
ret r
