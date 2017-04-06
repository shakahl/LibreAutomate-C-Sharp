out
 str s="$system$\Tasks\Quick Macros\QM - Macro2373"
 out FileExists(s)
 _s.getfile(s); err out _error.description
Dir d
foreach(d "$system$\Tasks\Quick Macros\QM - *" FE_Dir)
	str path=d.FullPath
	out path
	
