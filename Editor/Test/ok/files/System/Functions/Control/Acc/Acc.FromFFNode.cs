function FFNode'f [flags] ;;flags: 1 get of parent/ancestor if unavailable

 Gets Acc from FFNode, and initializes this variable.

 Added in: QM 2.3.3.


if(!f) end ERR_BADARG
a=0; elem=0

a=+f
err
	if flags&1
		rep
			f=f.node.parentNode; err break
			if(!f) break
			a=+f; err continue
			break

if(!a) end ERR_OBJECTGET


 info:
 To get ia from isdm, can use QI or QS. To get isdm from ia, need QS.
