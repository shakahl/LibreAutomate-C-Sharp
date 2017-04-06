out
TestANY 3
TestANY 3.5
TestANY "lssss"
str s="strrrr"
TestANY s
TestANY &s
str* p=&s; TestANY p
RECT r.right=100; TestANY r
TestANY &r
RECT& rr=r; TestANY rr
Acc a.elem=55; TestANY a
IAccessible aa; TestANY aa
int k=7; int& ir=k; TestANY ir

int w=win("" "Shell_TrayWnd")
Acc a1.Find(w "PUSHBUTTON" "" "class=MSTaskListWClass" 0x1005)
TestANY a1.a.Parent
TestANY SysAllocString(L"ttt")
TestANY RetVariant(0)

TestANY &sub.Callb
_i=&sub.Callb; TestANY _i

 out "%i" &sub.Callb
 VARIANT v=&sub.Callb; outx v.vt
 _s.all(1000 2); _snprintf _s _s.len "%i" &sub.Callb; out _s


#sub Callb
out __FUNCTION__
