 /
function# hwndParent $tag [$text] [$classname] [flags] [ctrlId]

 note: don't use this because also must remove prop

 Finds control previously tagged by TagControl. Returns its handle. If does not find, returns 0.

 tag - the same string that used with TagControl.
 other arguments - the same as with function child.

 See also: <TagControl>

 EXAMPLE
 int hwndParent=win("Calculator")
 int w1=id(126 hwndParent) ;;button "2"
 TagWindow w1 "b2"
 int w2=FindTaggedControl(hwndParent "b2")


ARRAY(int) a; int i
if(ctrlId) child(ctrlId text classname hwndParent flags 0 0 a)
else child(text classname hwndParent flags 0 0 a)

for(i 0 a.len) if(GetProp(a[i] tag)) ret a[i]

err+
