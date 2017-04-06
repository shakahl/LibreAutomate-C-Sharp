function str'number str&words

 Converts numeric currency to words.
 Error if number is invalid.
 number may begin or end with $, and may contain ,.

 EXAMPLES
 str sw
 SpellNumber 50845212.57 sw
 out sw
 SpellNumber "$2,512.96" sw
 out sw


number.trim("$ ")
number.findreplace(",")
int-- t_codeadded; if(!t_codeadded) t_codeadded=1; VbsAddCode "SpellNumberVbCode" 1
words=VbsFunc("SpellNumber" number)
err end _error
words.findreplace("  " " ")
