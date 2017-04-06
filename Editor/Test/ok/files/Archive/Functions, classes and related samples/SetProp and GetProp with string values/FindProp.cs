 /
function! hwnd $propw str&propFull

 Finds a window property.
 In propw you specify window property name with wildcard characters, eg "ab=*".
 In propFull you receive full property name, eg "ab=cd"
 Returns 1 if found, 0 if not.
 Note: This function is not to be used with properties set by SetPropString. Use GetPropString instead.

 EXAMPLE
 int h=win("Notepad")
 SetProp h "x=qwerty" 71
 str s
 if(FindProp(h "x=*" s))
	 out s
	 out GetProp(h s)


type FINDPROPDATA $propw str*propFull
propFull.all
EnumPropsEx hwnd &sub.EnumProc &propw
ret propFull.len!0


#sub EnumProc
function# hwnd $lpszString hData FINDPROPDATA&x

if(lpszString<=0xffff) ret 1 ;;atom
 out lpszString
if(!matchw(lpszString x.propw 1)) ret 1
*x.propFull=lpszString
