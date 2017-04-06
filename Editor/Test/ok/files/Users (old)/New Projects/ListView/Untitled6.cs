int hwnd=win("Favorites" "ExploreWClass")
int hlv=child("" "SyslistView32"  hwnd)
int Nitems
int* ItemIndex ;;function will allocate array

Nitems=GetSelListViewIndices(hlv &ItemIndex)
out Nitems
for(_i 0 Nitems) out ItemIndex[_i]
 ...
if(ItemIndex) free(ItemIndex)