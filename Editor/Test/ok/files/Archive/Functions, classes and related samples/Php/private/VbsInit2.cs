 /
function MSScript.ScriptControl&c $lang [$&src] [flags] [str&s]

#if EXE
#opt nowarnings 1
#endif

if(!c)
	c._create
	c.Language=lang
	c.Timeout=-1
	 c.AllowUI=0 ;;?

if(flags&1)
	s.getmacro(src)
	if(flags&4) s.getl(s 1 2)
	src=s
else if(flags&2) src=s.getfile(src)

err+ end _error
