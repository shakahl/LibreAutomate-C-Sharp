int hlv=child("" "SyslistView32" win("Favorites" "ExploreWClass"))

str s; int item=-1
rep
	if(GetListViewItemText2(hlv item &s 0 2 &item)=0) break
	out s