 out "src=%i, code=%i, iid=%i, place=%i" _error.source _error.code _error.iid _error.place
 out _error.description
 out _error.line

if(_error.source)
	str macroname.getmacro(getopt(itemid 3) 1)
	str functionname.getmacro(_error.iid 1)
	str s.format("Macro %s ended due to an error in %s.[]Error description: %s[]Error line: %s" macroname functionname _error.description _error.line)
	LogFile s 1
	out s
	