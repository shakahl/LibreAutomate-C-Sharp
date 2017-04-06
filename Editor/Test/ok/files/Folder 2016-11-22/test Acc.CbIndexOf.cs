int w=wait(3 WV win("Quick Macros Forum * - Internet Explorer" "IEFrame" "" 1))
act w
Acc a.Find(w "COMBOBOX" "Jump to:" "" 0x3001 3)

a.Select(1)
int n=a.CbIndexOf("*My QM"); if(n<0) end "combo box item not found"
key V ;;Space should show the list
key H D(#n) Y
