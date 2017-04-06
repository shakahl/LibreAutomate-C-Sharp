out
int w=wait(2 WV win("Mozilla Firefox Start Page - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
Acc a.FindFF(w "a" "Sign up" "" 0x1001 2)
 act w

 a.DoDefaultAction

 a.Select(1)

 int x y cx cy
 a.Location(x y cx cy)
 out F"{x} {y} {cx} {cy}"

 str name=a.Name
 out name

 str value=a.Value
 out value

 a.SetValue("fffffff")

 str descr=a.Description
 out descr

 str roleStr
 int roleInt=a.Role(roleStr)
 out F"{roleInt} {roleStr}"

 str stateText
 int stateFlags=a.State(stateText)
 out F"0x{stateFlags} {stateText}"

 int w1=win("Quick Macros" "QM_Editor")
 Acc a1.Find(w1 "OUTLINE" "" "class=SysTreeView32[]id=2202" 0x1004)
 Acc aFocus
 a1.ObjectFocused(1 aFocus)
 out aFocus.Name
 
 ARRAY(Acc) aSel
 a1.ObjectSelected(aSel)
  sample code, shows how to use the array
 for _i 0 aSel.len
	 out aSel[_i].Name

 a.Mouse(1)
 a.Mouse(1 4 (5 + 1))

 a.CbSelect("")
 a.CbSelect(0)

 a.WebScrollTo

 str href=a.WebAttribute("href")
 out href

 str tag innerHTML outerHTML
 a.WebProp(tag innerHTML outerHTML)
 out F"'{tag}'[]'{innerHTML}'[]'{outerHTML}'"

str url title html text
a.WebPageProp(url title html text)
out F"'{url}'[]'{title}'[]'{html}'[]'{text}'"
ret
