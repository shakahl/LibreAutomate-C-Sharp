out
int w
run "$system$\taskschd.msc" "" "" "" 0xB00 win("Task Scheduler" "MMCMainFrame") w
int c=id(12785 w) ;;treeview

int htvi htvi2
htvi=SendMessage(c TVM_GETNEXTITEM 0 0)
#compile "__WindowsControls"
WindowsControls x
htvi=x.TreeView_FindItem(c "Task Scheduler Library" htvi)
rep ;;wait
	SendMessage(c TVM_EXPAND TVE_EXPAND htvi)
	htvi2=x.TreeView_FindItem(c "Test" htvi)
	if(htvi2) break
	0.1
SendMessage(c TVM_SELECTITEM TVGN_CARET htvi2)

Acc a.Find(w "LISTITEM" "n" "class=*.SysListView32.*[]wfName=listViewMain" 0x1005 30)
a.DoDefaultAction
key Mp

 tested: works even if tree hidden.

 Other ways to show task properties don't work or unreliable:
 a.DoDefaultAction ;;just selects
 a.Select(2) ;;no
 int w1=win("ACFx:CxtMenuSink" "ATL:5C5A3220"); men 1017 w1 ;;Properties ;;no
 key A{ap} ;;menu may be hidden
 Click acc Actions -> Properties ;;may be hidden
 a.Mouse(4); mou ;;unreliable, visible
