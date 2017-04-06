 win "" "" "" 0 F"callback={&sub.CbW}"
 
 #sub CbW
 function# hwnd cbParam
 outw hwnd
  min 0
 child "" "" hwnd 0 F"callback={&sub.CbC}"
 ret 1
 
 #sub CbC
 function# hwnd cbParam
 outw hwnd
 min 0
 ret 1


mes F"<><a href=''{&sub.Link}''>link text</a>"
 inp _s F"<><a href=''{&sub.Link}''>link text</a>"
 
 
#sub Link
function# hwnd $params
outw hwnd
min 0

 ICsv x._create; x.FromString(",,2[]one[]two")
 ShowDropdownList(x 0 0 0 0 0 &sub.CbSDL)
 
 #sub CbSDL
 function# cbParam QMDROPDOWNCALLBACKDATA&x
 out 1
 min 0
