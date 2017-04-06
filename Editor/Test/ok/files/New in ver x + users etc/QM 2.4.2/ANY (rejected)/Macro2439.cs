out
 int i=5; TestANY i
 double d=5.5; TestANY d
 lpstr ls="lp"; TestANY ls
 str s="ss"; TestANY s
 int* p=&i; TestANY p
 TestANY &i

 out "%i" p
 _s.all(1000 2)
 _snprintf _s _s.len "%i" p
 out _s

 TestANY "ddd"
 Acc a
 ARRAY(str) a
 TestANY a

 TestANY SysAllocString(L"ttt")
 TestANY RetArrayStr
 TestANY RetVariant(2)

 mac "TestANY" "" 5

 int i=5; int* p=&i; TestANY *p

 str s="str"
 ARRAY(str) a="arrrrrray"
 IUnknown u
 int w=win("Start" "Button")
 Acc a1.Find(w "PUSHBUTTON" "Start" "" 0x1001)
 RECT r
 str* p=&s; str** pp=&p; str*** ppp=&pp
 
 TestANY2 4 6.7 "string" s a a1 a1.a u r SysAllocString(L"ttt") RetVariant(2) RetArrayStr &s &a p pp ppp "end"

 TestANY sub.RetDouble
 TestANY GetTickCount

 ANY x.ta=1; x.ts="int"; x.i=8; TestANY2 x &x

 TestANY win("" "QM_Editor")

 TestANY RetStr(1)
 TestANY _s.from("A" 5)
 BSTR b; TestANY b.add("bbb" 7)
 TestANY2 acc(mouse) acc("Start" "PUSHBUTTON" w)

 TestANY2 perf perf perf perf perf perf perf perf
 ARRAY(str) ae; ARRAY(str) as="strrr"; ARRAY(lpstr) asl; asl[]="LL"; asl[]="LL"; ARRAY(BSTR) ab="bstrrr"; ARRAY(POINT) ap.create(2)
 TestANY2 sizeof(RECT) uuidof(IAccessible) ae as asl ab ap
 GUID g=*uuidof(IDispatch); TestANY2 g

 RECT r; SetRect r 2 3 4 50; POINT p.x=3; p.y=8; DateTime dt.FromComputerTime
 TestANY2 r p dt

 byte b=1; word w=2; long l=3; FLOAT f=4.4; CURRENCY c=5.5; DECIMAL d=6.6; VARIANT v=7.7; CHARRANGE cr.cpMin=8; cr.cpMax=9
 DATE da.getclock; FILETIME ft; da.tofiletime(ft); SYSTEMTIME st; da.tosystemtime(st)
 CALLOUT co; _s.getfile("hhhhhhhhhhhhhhhhhhhhh"); err
 TestANY2 b w l f c d v cr da ft st 2L co _error

int i=1; str s="string"
out _s.Join(", " s i 2.2)

 str s="str"
 out s.Join(" " 1 s 7)

 out _s.Join("[]" 1 2 3 "ssss")
 out _s.Join(0 1 "[]" 2 3 "ssss")

 lpstr s="abc"
 TestANY2 1+2 1+2.2 s+1 123456789012345 1+123456789012345 0xF2345678 0x12345678 -0x12345678 -0xF2345678 -5
 out "-------"
 out -0x12345678
 out "%i %i %i" 0x12345678 -0x12345678 111

 str s1 s2
 TestANY2 RetStr(1)+2 s1.from("kkkk")+2 s2.from("mmm") win(mouse)+4

 TestANY1 @"bbbb" @ "sss"
 PF
 rep(1000) TestANY4 1
 PN
 rep(1000) TestANY5 1
 PN
 rep(1000) TestVARIANT30 1
 PN;PO

 PF
 rep(1000) sub.TestVARIANT 5
 PN
 rep(1000) sub.TestVARIANT "string dhhhhhhhhhhhhhhhhhhhhhhhhhhhh"
 PN
 rep(1000) sub.TestANY 5
 PN
 rep(1000) sub.TestANY "string dhhhhhhhhhhhhhhhhhhhhhhhhhhhh"
 PN
 rep(1000) sub.TestInt 5
 PN
 rep(1000) sub.TestStr "string dhhhhhhhhhhhhhhhhhhhhhhhhhhhh"
 PN
 PO
 
 ANY k=TestANY6(5)
 out k.ts
 
 #sub TestVARIANT
 function VARIANT'x
 
 
 #sub TestANY
 function ANY'x
 
 
 #sub TestInt
 function i
 
 
 #sub TestStr
 function ~s

#sub RetDouble
function'double
ret 5.9
