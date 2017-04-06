 /PLA_Main
function

PLA_DATA- d
PLA_Control 0
d.a=0
SendMessage d.hlv LVM_DELETEALLITEMS 0 0

str s.getwintext(id(12 d.hdlg))
rset s "folder" "\Examples\QM MP3 Player"
if(!s.len or !dir(s 1)) ret

s+"\*.mp3"
Dir di
foreach(di s FE_Dir 0x4)
	str sPath=di.FileName(1)
	 out sPath
	TO_LvAdd d.hlv -1 0 0 sPath
	d.a[]=sPath

TO_LvAdjustColumnWidth d.hlv 1
