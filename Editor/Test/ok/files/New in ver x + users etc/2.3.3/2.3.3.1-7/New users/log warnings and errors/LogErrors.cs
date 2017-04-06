 if(_error.source) ;;macro ended due to an error
	 str macroname.getmacro(getopt(itemid 3) 1)
	 str functionname.getmacro(_error.iid 1)
	 str s.format("Macro %s ended due to an error in %s.[]Error description: %s[]Error line: %s[]" macroname functionname _error.description _error.line)
	 out s
	 out _error.place

 ...

ICsv- t_logErr=CreateCsv(1)

 then append error or success, like in LogWarning

 then append all to file
lock 0 "QM_mutex_LogErrors"
t_logErr.ToFile("$desktop$\errorlog.csv" 1)
