 /test Java Access Bridge
function# vmID JAB.AccessibleText'at [str&s]

 Gets object text.
 Stores text in variable s and returns text length in characters (not bytes). Returns 0 if text empty or it is object that does not have text property or if error. If s not used, returns text length.
 Type of at also can be JAB.AccessibleContext or JOBJECT64.


if(&s) s.fix(0)
JAB.AccessibleTextInfo t
if(!JAB.GetAccessibleTextInfo(vmID at &t 0 0)) ret
 out t.charCount
if(!&s) ret t.charCount
if(!t.charCount) ret
int n=t.charCount+100
if(n>0xffff) n=0xffff; if(t.charCount>n) t.charCount=n
BSTR b.alloc(n)
if(!JAB.GetAccessibleTextRange(vmID at 0 t.charCount-1 b.pstr n)) ret
s=b
ret t.charCount
