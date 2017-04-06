str s
int w=wait(2 WV win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
Acc a1.FindFF(w "IMG" "" "" 0 0 0 "" &Acc_FindFF_callback_get_img_src &s)
 out s

if(!s.len) ret
int i=ShowMenu(s 0 0 2)-1; if(i<1) ret
s.getl(s i)
out s

 str url; a1.WebPageProp(url) ;;use this if need page URL
