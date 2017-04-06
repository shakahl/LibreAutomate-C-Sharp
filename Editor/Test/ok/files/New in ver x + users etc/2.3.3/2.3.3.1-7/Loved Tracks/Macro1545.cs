lef
key HSE
str s.getsel
act "YouTube"
 ----
int w=act(win("YouTube" "Mozilla*WindowClass" "" 0x804))
Acc a.FindFF(w "INPUT" "" "title=Search[]name=search_query[]id=masthead-search-term" 0x1004 2)
a.Select(1)
key Ca
s.setsel
key Y
