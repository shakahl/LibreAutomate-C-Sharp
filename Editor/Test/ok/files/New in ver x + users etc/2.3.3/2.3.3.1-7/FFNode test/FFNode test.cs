 This macro will be displayed when you click this class name in the editor and press F1.
 Add class help here. Add code examples below EXAMPLES.

 EXAMPLES

out
int w1=win("- Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
FFNode x
Acc a

x.FindFF(w1 "TABLE" "" "" 0x1000)
 x.FindFF(w1 "1 A" "" "" 0x1000)
 _s=x.HTML; if(!_s.len) _s=x.Text
 out _s
 x.FindFF(w1 "A" "" "" 0x1000)
 x.FindFF(w1 "SPAN" "" "" 0x1000)
 out x.HTML
 a.FromFFNode(a 1)
 out a.Name
 x.FindFF(w1 "A" "SVG" "" 0x1001)
 x.FindFF(w1 "A" "^SV" "" 0x1002)

str attr=
 class=ex-ref
 href=http://www.w3.org/Graphics/SVG/Overview.html
 
 x.FindFF(w1 "A" "" attr 0x1000)
 x.FindFF(w1 "A" "" attr 0x1004)
 x.FindFF(w1 "A" "" attr 0x1008)


 x.FindFF(w1 "A" "" "" 0x1000 0 0 &FFNode_callback 0)
 int ntags
 x.FindFF(w1 "" "" "" 0 0 0 &FFNode_callback &ntags)
 out ntags

 FFDOM.ISimpleDOMNode g
 FFNode gg
 a=acc("DHTML and AJAX : Gecko and other browsers have long supported dynamic content, where the page appearance changes because of JavaScript. This can be used to create the appearance of desktop-style widgets like menus, spreadsheets and tree views which HTML lacks. Or, it can be used to completely change content on the fly, without loading a new page. Previously it was not posible to make this accessible, but Firefox 1.5 supports Accessible DHTML , which allows authors to make advanced widgets and web applications accessible." "LISTITEM" win("Gecko Info for Windows Accessibility Vendors - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804) "" "" 0x3091)
 x.FindFF(a.a "A" "" "" 0x1000 0 2)
 out x.HTML
 x.FindFF(x "" "dh")
 out x.Text


 type MYFFTYPE str's ARRAY(FFNode)a
 MYFFTYPE y
 y.s="a"
 FFNode fn.FindFF(w1 "A" "" "" 0 0 0 &FFNode_Find_callback2 &y)
 int i
 for(i 0 y.a.len) out y.a[i].HTML

 x.FindFF(w1 "tree" "" "" 0x3000); err out "not found"
 x.FindFF(w1 "#document")
 out x
 FFNode ff.FindFF(w1 "#document")
 out ff
