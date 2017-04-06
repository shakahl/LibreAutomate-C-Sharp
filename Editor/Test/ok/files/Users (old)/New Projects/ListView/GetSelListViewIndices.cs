 /
function# hlv *&ItemIndex

 Returns number of selected items and their Indices in an array

 EXAMPLE
 int hwnd=win("Favorites" "ExploreWClass")
 int hlv=child("" "SyslistView32"  hwnd)
 int Nitems
 int* ItemIndex ;;function will allocate array
 
 Nitems=GetSelListViewIndices(hlv &ItemIndex)
 out Nitems
 for(int'i 0 Nitems) out ItemIndex[i]
  ...
 if(ItemIndex) free(ItemIndex)


if(hlv=0 or &ItemIndex=0) end "invalid argument"
int n=SendMessage(hlv LVM_GETSELECTEDCOUNT 0 0); if(n=0) ret
ItemIndex = realloc(ItemIndex n*4)

int i it=-1
for(i 0 n)
	it=SendMessage(hlv LVM_GETNEXTITEM it 2)
	if(it>=0) ItemIndex[i]=it; else break

ret i
	