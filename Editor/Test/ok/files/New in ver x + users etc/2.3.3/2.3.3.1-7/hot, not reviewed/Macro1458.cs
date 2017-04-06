out
HtmlDoc d.InitFromInternetExplorer(win("" "IEFrame"))
ARRAY(str) a
d.GetTable("AssignmentActivityHistory_dgAssignmentActivity" a)
int i
for i 0 a.len 5
	out a[i+4] ;;5-th column
