function! $_word

 Checks if word is correct.
 Returns 1 if correct, 0 if no.


if(!m_speller) end ES_INIT
if(empty(_word)) ret 1
int correct=aspell.aspell_speller_check(m_speller _word -1)
ret correct!0
