 /
function# $classNameEtc [iFrom]

 Open "Type Libraries" dialog and call this function.
 Selects each typelib in the list and gets text from the content control.
 If finds classNameEtc in it, selects the typelib and returns its index.
 Returns -1 if not found.

int w=win("Type Libraries" "#32770")
act w
int c1=id(1202 w) ;;list
int c2=id(1574 w) ;;QM_DlgInfo
Acc a.Find(w "LIST" "" "class=SysListView32[]id=1202" 0x1004)
int i n=a.ChildCount
for i iFrom+1 n
	a.elem=i
	a.Select(3)
	str s
	SciGetText c2 s
	 out "-----"
	 out s
	if(findw(s classNameEtc)>=0)
		SendMessage c1 LVM_ENSUREVISIBLE i 0
		ret i
ret -1
