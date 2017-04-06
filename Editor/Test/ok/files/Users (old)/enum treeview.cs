Acc a=acc("" "OUTLINE" "+ExploreWClass" "SysTreeView32" "" 0x1000) ;;captured with Find Accessible Object dialog
Acc a1
str s ss
int level
a.Navigate("f" a1) ;;first item
rep
	s=a1.Name
	ss=a1.Value
	ss.set(9 0 val(ss))
	out "%s%s" ss s
	a1.Navigate("n"); err break
