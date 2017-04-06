function [$lang] [$options] [$wordlists_folder]

 Initializes the variable.
 Call this function before calling other Caspell functions.
 Error if fails. For example, if the dictionary for the language is not installed.

 lang - language. Default: "en_US" (American English). Must be the standard two letter ISO 639 language code, with an optional two letter ISO 3166 country code after an underscore. The dictionary must be installed.
 options - list of Aspell options. Line format: "name value" or "name=value". Aspell options are documented in Aspell manual, which is installed with Aspell (look in Start menu). Error if some option name or value is invalid or the option cannot be changed.
 wordlists_folder - folder of personal word lists. Required if you'll use SaveWordLists, because by default it is Aspell folder which normally is in Program Files and therefore data files there cannot be saved.


Clear
m_config=aspell.new_aspell_config; err end _error
if(!m_config) end ES_FAILED
if(empty(lang)) lang="en_US"
if(!aspell.aspell_config_replace(m_config "lang" lang)) goto ge1
if(_unicode and !aspell.aspell_config_replace(m_config "encoding" "utf-8")) goto ge1
if(!empty(wordlists_folder))
	str wlf.expandpath(wordlists_folder)
	mkdir wlf; err end _error
	if(!aspell.aspell_config_replace(m_config "personal" _s.from(wlf "\" GetOption("personal")))) goto ge1
	if(!aspell.aspell_config_replace(m_config "repl" _s.from(wlf "\" GetOption("repl")))) goto ge1

if(!empty(options))
	str s sn sv
	foreach s options
		if(tok(s &sn 2 " =" 2)<2) end ES_BADARG
		if(!aspell.aspell_config_replace(m_config sn sv)) goto ge1

byte* possible_err=aspell.new_aspell_speller(m_config)
if(aspell.aspell_error_number(possible_err)) end aspell.aspell_error_message(possible_err)
else m_speller=aspell.to_aspell_speller(possible_err)
ret
 ge1
end aspell.aspell_config_error_message(m_config)
