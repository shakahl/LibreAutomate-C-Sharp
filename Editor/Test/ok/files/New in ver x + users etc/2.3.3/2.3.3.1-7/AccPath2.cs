 /Macro1477
function'Acc hwnd $path $name [flags]



type AP_PROC Acc'aret ARRAY(str)ap ip $name flags
AP_PROC d

d.name=name
d.flags=flags
d.ip=1

if(tok(path d.ap -1 "/''" 4)<2) end ERR_BADARG

acc("" d.ap[0] hwnd "" "" flags&(16|32|128)|64|0x8000 &AP_Proc &d)
if(d.aret.a) ret d.aret
 if(flags&0x1000) end ES_OBJECT
