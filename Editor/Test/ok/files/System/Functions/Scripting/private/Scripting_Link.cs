 \

 _command:
 line column flags "source"
 source depends on flags&3:
   0 - code was in variable. Currently does not create link.
   1 - "file" (may be not the main file)
   2 - "macro"

 out _command

str s1 s2 s3 s4
if(4!tok(_command &s1 4 " ''" 4)) ret
int line(val(s1)-1) column(val(s2)-1) flags(val(s3)) sourceType(flags&3)

sel sourceType
	case 1
	s1.getfilename(s4 1)
	QMITEM q; int iid=qmitem(s1 1 q 0x100); if(iid) q.linktarget.expandpath; if(!(q.linktarget~s4)) iid=0
	if(iid) mac+ iid; else newitem s1 s4 "File Link" "" "" 4|128
	
	case 2
	mac+ s4
	
	case else ret

int h=GetQmCodeEditor

 int i=SendMessage(h SCI.SCI_FINDCOLUMN line column) ;;problem with tabs
 SendMessage h SCI.SCI_GOTOPOS i 0

SendMessage h SCI.SCI_GOTOLINE line 0
rep(column) SendMessage h SCI.SCI_CHARRIGHT 0 0

err+
