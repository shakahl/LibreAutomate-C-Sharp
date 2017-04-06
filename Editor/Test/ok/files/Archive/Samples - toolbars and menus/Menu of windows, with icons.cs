 Shows dynamically created menu of currently open windows.

str titles
ARRAY(int) handles
GetWindowList &titles "" 1|2|4 0 0 handles
ARRAY(str) arr = titles
ARRAY(int) hicons.create(handles.len)
int i

for(i 0 arr.len)
	hicons[i]=GetWindowIcon(handles[i])
	arr[i].findreplace(":" "[91]58]") ;;escape :
	arr[i].escape(1) ;;escape "
	arr[i].formata(" : * %i" hicons[i])

titles=arr

int p=DynamicMenu(titles "" 1)

for(i 0 arr.len) DestroyIcon(hicons[i])

if(p) act handles[p-1]
