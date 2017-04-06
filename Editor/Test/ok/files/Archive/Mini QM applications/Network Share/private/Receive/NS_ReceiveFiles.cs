 \
function# str'filedata

lpstr zf="$temp$\qm_ns_receive.zip"
lpstr folder="$my qm$\received files"

if(filedata.len<4 or !filedata.beg("ns")) ret ;;check signature
int pc=filedata[2]
filedata.get(filedata 4)
sel pc
	case 255 ;;begin
	if(dir(zf)) del zf
	
	case 254 ;;end
	zip- zf folder 1
	del- zf; err
	run folder "" "" "" 0x100 "received files"
	_s.format("Received files(s) from %s." filedata)
	OnScreenDisplay _s
	
	case 253 ;;cancel
	del- zf; err
	
	case else ;;data
	if(pc>100) ret
	filedata.setfile(zf -1) ;;append
	ret 1

err+ ret
ret 1
