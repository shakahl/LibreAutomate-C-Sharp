 /
function! hwnd $name str&value

 Gets a string property assigned to the window using SetPropString.
 Similar to GetProp, but GetProp gets an integer property.
 Returns 1 if found, 0 if not.

 EXAMPLE
 int h=win("Notepad")
 SetPropString h "y" "asdfg"
 str s
 if(GetPropString(h "y" s))
	 out s


str s1.from(name "[1]*")
if(FindProp(hwnd s1 value))
	value.get(value findc(value 1)+1)
	ret 1
