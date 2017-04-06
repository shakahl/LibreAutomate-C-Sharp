 \
function# $text [$tempitemname] [sync]

 Shows popup menu created at run time.
 If sync is omitted or 0, does not wait, and returns 0.
 If sync is nonzero, waits and returns 1-based index of selected line.

 text - menu text. Same as in usual popup menus.
 tempitemname - name of temporary popup menu. Default: "temp_menu".

 See also: <ShowMenu>.

 EXAMPLE
 out DynamicMenu("1 :mes 1[]2 :mes 2" "" 1)


opt noerrorshere 1

if(empty(tempitemname)) tempitemname="temp_menu"

if(sync) ret mac(newitem(tempitemname text "Menu" "" "" 1|128))
else mac newitem(tempitemname text "Menu" "" "" 1|128)
