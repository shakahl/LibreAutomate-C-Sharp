
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "`OK"
 2 Button 0x54030000 0x4 168 116 48 14 "`Cancel" "`ttCancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

ARRAY(lpstr) aLang.create(2)
lpstr lang

lang=
 OK, OK 1
 Cancel, Cancel 1
 ttCancel, Tooltip 1
aLang[0]=lang

lang=
 OK, OK 2
 Cancel, Cancel 2
 ttCancel, Tooltip 2
aLang[1]=lang

int iLang=ListDialog("Language 1[]Language 2")-1
if(iLang<0) ret

IStringMap m._create
m.AddList(aLang[iLang] "csv")
REPLACERX r.frepl=&sub.Callback_str_replacerx; r.paramr=&m
dd.replacerx("(?<='')(`.+?)(?='')" r)
 out dd

if(!ShowDialog(dd 0 0)) ret


#sub Callback_str_replacerx
function# REPLACERXCB&x

IStringMap& m=+x.rrx.paramr
lpstr repl=m.Get(x.match+1)
if(repl) x.match=repl
