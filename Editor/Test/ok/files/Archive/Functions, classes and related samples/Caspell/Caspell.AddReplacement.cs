function $misspelled_word $replacement

 Stores replacement to the personal replacements word list that is associated with current dictionary.
 Next time it will appear somewhere near the top of the list of suggestions for the misspelled word.
 To save it, call SaveWordLists. Else it be valid only with this instance of this variable.


if(!m_speller) end ES_INIT
aspell.aspell_speller_store_replacement(m_speller misspelled_word -1 replacement -1)
