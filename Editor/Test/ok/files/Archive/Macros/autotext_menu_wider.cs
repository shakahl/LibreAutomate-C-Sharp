 If need, makes wider the popup list of autotext items which is shown when multiple items match typed text.
 Runs when the popup list is shown. Need to assign this trigger: window created & visible, class QM_PopupList.

int w=TriggerWindow

int c=id(8888 w)
int wid1=SendMessage(c LVM_GETCOLUMNWIDTH 0 0)
SendMessage(c LVM_SETCOLUMNWIDTH 0 LVSCW_AUTOSIZE)
int wid2=SendMessage(c LVM_GETCOLUMNWIDTH 0 0)
if wid2<=wid1
	SendMessage(c LVM_SETCOLUMNWIDTH 0 wid1)
	ret

RECT r; GetWindowRect w &r
int plus=r.right-r.left-wid1
int wid3=wid2+plus

 limit in work area
int xWA widWA; GetWorkArea xWA 0 widWA 0 0 w; xWA+100; widWA-200
int dif=wid3-widWA
if dif>0
	wid3-dif; wid2-dif
	SendMessage(c LVM_SETCOLUMNWIDTH 0 wid2)
r.right=r.left+wid3
dif=r.right-(xWA+widWA); if(dif>0) r.left-dif; r.right-dif

MoveWindow w r.left r.top r.right-r.left r.bottom-r.top 1
GetClientRect w &r
MoveWindow c 0 0 r.right r.bottom 1
