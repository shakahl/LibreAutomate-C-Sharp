_monitor=-1
OnScreenDisplay "click the item in 10 seconds"
wait 10 ML; err ret
OsdHide
int hlv=child(mouse)
Acc a=acc("" "LIST" hlv "" "" 0x1000); err ret
ARRAY(Acc) aa
a.Selection(aa)
if(!aa.len) ret
a=aa[0]
a.Mouse(2)
wait 5 WV "+#32768"
key rrrRp
0.5
a.Select(2) ;;now a is first element after deleted element
err ret ;;last item
SendMessage hlv LVM_ENSUREVISIBLE a.elem 0
