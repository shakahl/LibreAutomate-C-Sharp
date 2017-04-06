out

 tested: none of these functions look in registry.

 str s="q:\app\catkeys\..\qmcl.exe"
 str s="qmcl.exe"
 str s="notepad.exe"
 str s="no tepad.exe"
 str s="n:\tepad.exe"
 str s="http://www.a.d"
str s="wordpad.exe"

 str b.all(300)
  out PathSearchAndQualify(s b 300)
 
 b.flags=1
 b=s
 out b.nc
 ARRAY(lpstr) a.create(2); a[0]="C:\Program Files (x86)\Windows NT\Accessories"
  out PathFindOnPath(b &a[0])
 
 b.fix
 out b

BSTR b.alloc(300)
BSTR t=s
memcpy b.pstr t.pstr t.len*2+2
out PathResolve(b 0 PRF_VERIFYEXISTS|PRF_REQUIREABSOLUTE)
out b
