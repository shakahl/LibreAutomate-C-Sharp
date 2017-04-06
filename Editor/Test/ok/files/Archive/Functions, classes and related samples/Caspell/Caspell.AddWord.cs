function $_word

 Adds word to the personal word list that is associated with current dictionary.
 It should be a correct word that is not in the main dictionary.
 To save it, call SaveWordLists. Else it be valid only with this instance of this variable.


if(!m_speller) end ES_INIT
aspell.aspell_speller_add_to_personal(m_speller _word -1)
