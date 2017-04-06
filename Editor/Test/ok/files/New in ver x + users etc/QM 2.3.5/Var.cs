 /
function` `v vt

if(vt=v.vt) ret v

VARIANT r
int hr=VariantChangeTypeEx(&r &v 0x409 0 vt)
if(hr) end F"Cannot change type. {_s.dllerror(`` `` hr)}"
ret r
