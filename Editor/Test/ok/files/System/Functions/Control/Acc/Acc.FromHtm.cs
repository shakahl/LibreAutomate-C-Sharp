function Htm'el [flags] ;;flags: 1 get of parent/ancestor if unavailable

 Gets Acc from Htm, and initializes this variable.

 Added in: QM 2.3.3.


if(!el) end ERR_BADARG
a=0; elem=0

if(flags&1) this=acc(el); err
else a=sub_sys.QueryService(el uuidof(IAccessible))

if(!a) end ERR_OBJECTGET
