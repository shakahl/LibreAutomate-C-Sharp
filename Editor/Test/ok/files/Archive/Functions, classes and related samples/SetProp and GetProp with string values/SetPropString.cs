 /
function hwnd $name $value

 Assigns a string property to the window. Or changes.
 Similar to SetProp, but SetProp assigns an integer property.


str s1.from(name "[1]*") s2
if(FindProp(hwnd s1 s2)) RemoveProp hwnd s2
s1.from(name "[1]" value)
SetProp hwnd s1 1
