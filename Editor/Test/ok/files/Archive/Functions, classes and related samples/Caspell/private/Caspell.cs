ref aspell aspell_def
class Caspell -!*m_config -!*m_speller

str s
if(!GetEnvVar("aspell.dll" s))
	rget s "path" "software\aspell" HKEY_LOCAL_MACHINE "$pf$\aspell\bin"
	s+"\aspell-15.dll"
	SetEnvVar "aspell.dll" s
