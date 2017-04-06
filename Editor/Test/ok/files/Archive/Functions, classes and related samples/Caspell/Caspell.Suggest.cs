function $_word ARRAY(str)&suggestions

 Populates suggestions array with suggested words.


if(!m_speller) end ES_INIT
suggestions=0
if(empty(_word)) ret
byte* sug=aspell.aspell_speller_suggest(m_speller _word -1); if(!sug) ret
byte* elem=aspell.aspell_word_list_elements(sug)
lpstr w
rep
	w=aspell.aspell_string_enumeration_next(elem)
	if(!w) break
	suggestions[]=w
aspell.delete_aspell_string_enumeration(elem)
