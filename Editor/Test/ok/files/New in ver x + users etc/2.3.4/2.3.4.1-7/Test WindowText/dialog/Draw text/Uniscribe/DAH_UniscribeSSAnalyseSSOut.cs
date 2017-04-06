 /dlg_apihook
function hdc str's CLogicalCoord&c [flags] [ansiCharset] ;;flags: 1 ANSI, 2 minimal

int hr
byte* ssa
BSTR b=s

if(flags&1) hr=WINAPI2.ScriptStringAnalyse(hdc +s s.len 2*s.len+16 ansiCharset WINAPI2.SSA_GLYPHS|WINAPI2.SSA_LINK 0 0 0 0 0 0 &ssa)
else hr=WINAPI2.ScriptStringAnalyse(hdc +b b.len 2*b.len+16 -1 WINAPI2.SSA_GLYPHS|WINAPI2.SSA_LINK 0 0 0 0 0 0 &ssa)
 out hr
hr=WINAPI2.ScriptStringOut(ssa c.x c.y 0 +&c 0 0 0)
if(flags&3) goto g1

  draw several times with same data
 c.Offset(0 15); hr=WINAPI2.ScriptStringOut(ssa c.x c.y 0 +&c 0 0 0)

  use other data without releasing old data
 byte* ssa2
 hr=WINAPI2.ScriptStringAnalyse(hdc +b b.len 2*b.len+16 -1 WINAPI2.SSA_GLYPHS|WINAPI2.SSA_LINK 0 0 0 0 0 0 &ssa2)
 c.Offset(0 15); hr=WINAPI2.ScriptStringOut(ssa2 c.x c.y 0 +&c 0 0 0)
 WINAPI2.ScriptStringFree(&ssa2)

 g1
WINAPI2.ScriptStringFree(&ssa)
if(flags&3) ret

 set iReqWidth for clipping or fitting
b="ABCDEFGHIJKLMNO"
hr=WINAPI2.ScriptStringAnalyse(hdc +b b.len 2*b.len+16 -1 WINAPI2.SSA_GLYPHS|WINAPI2.SSA_CLIP 30 0 0 0 0 0 &ssa)
c.Offset(0 20); hr=WINAPI2.ScriptStringOut(ssa c.x c.y 0 +&c 0 0 0); WINAPI2.ScriptStringFree(&ssa)
b="A B C D"
hr=WINAPI2.ScriptStringAnalyse(hdc +b b.len 2*b.len+16 -1 WINAPI2.SSA_GLYPHS|WINAPI2.SSA_FIT 200 0 0 0 0 0 &ssa)
c.Offset(0 20); hr=WINAPI2.ScriptStringOut(ssa c.x c.y 0 +&c 0 0 0); WINAPI2.ScriptStringFree(&ssa)

 tabs
b="A[9]B[9]C[9]D"
hr=WINAPI2.ScriptStringAnalyse(hdc +b b.len 2*b.len+16 -1 WINAPI2.SSA_GLYPHS|WINAPI2.SSA_TAB 0 0 0 0 0 0 &ssa)
c.Offset(0 20); hr=WINAPI2.ScriptStringOut(ssa c.x c.y 0 +&c 0 0 0); WINAPI2.ScriptStringFree(&ssa)

 hotkey
b="&Hotkey && co"
hr=WINAPI2.ScriptStringAnalyse(hdc +b b.len 2*b.len+16 -1 WINAPI2.SSA_GLYPHS|WINAPI2.SSA_HOTKEY 0 0 0 0 0 0 &ssa)
c.Offset(0 20); hr=WINAPI2.ScriptStringOut(ssa c.x c.y 0 +&c 0 0 0); WINAPI2.ScriptStringFree(&ssa)

 password
b="-/"
hr=WINAPI2.ScriptStringAnalyse(hdc +b 5 2*b.len+16 -1 WINAPI2.SSA_GLYPHS|WINAPI2.SSA_PASSWORD 0 0 0 0 0 0 &ssa)
c.Offset(0 20); hr=WINAPI2.ScriptStringOut(ssa c.x c.y 0 +&c 0 0 0); WINAPI2.ScriptStringFree(&ssa)
hr=WINAPI2.ScriptStringAnalyse(hdc +b 5 2*b.len+16 ANSI_CHARSET WINAPI2.SSA_GLYPHS|WINAPI2.SSA_PASSWORD 0 0 0 0 0 0 &ssa)
c.Offset(0 20); hr=WINAPI2.ScriptStringOut(ssa c.x c.y 0 +&c 0 0 0); WINAPI2.ScriptStringFree(&ssa)
