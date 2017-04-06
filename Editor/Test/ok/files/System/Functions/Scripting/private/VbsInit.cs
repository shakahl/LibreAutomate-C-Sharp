 /
function# MSScript.ScriptControl&c $lang [$&src] [flags] [str&ss]

 Initializes thread variable c (once).
 If src used, and initially it is macro or file, gets code (stores in ss and sets src).
 If macro, returns QM item id, else 0.

#opt nowarnings 1
opt noerrorshere 1

if !c
	c._create
	c.Language=lang
	c.Timeout=-1
	 c.AllowUI=0 ;;?

if(!&src) ret
if(flags&1) ;;fbc
	ss.getmacro(src)
	if(flags&4) ss.getl(ss 1 2)
	src=ss
else if(flags&2)
	src=ss.getfile(src)
else
	ret Scripting_GetCode(src ss 0 3)
