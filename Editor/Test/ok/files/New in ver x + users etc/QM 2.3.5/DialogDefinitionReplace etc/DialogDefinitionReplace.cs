 /
function what str&dd ctrlID v1 v2 ;;what: 0 style/exstyle, 1 x/y, 2 width/height

 Replaces two numeric values in dialog definition string.

 dd - variable containing dialog definition (BEGIN DIALOG...END DIALOG).
 ctrlID - control id, or 0 for dialog. It's the first number in a line.
 v1, v2 - new values. Coordinates are in dialog units.


str rx
sel what
	case 0 rx=F"^({ctrlID} .+?) 0x\w+ \w+"
	case 1 rx=F"^({ctrlID} .+? 0x\w+ \w+) \d+ \d+"
	case 2 rx=F"^({ctrlID} .+? 0x\w+ \w+ \w+ \w+) \d+ \d+"
	case else end ERR_BADARG

if(dd.replacerx(rx F"$1 {v1} {v2}" 4|8)<=0) end ERR_FAILED
