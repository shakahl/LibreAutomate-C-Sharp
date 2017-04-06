function ec

str regexError.all(4000)
if(pcre2_get_error_message(ec regexError 4000)<=0) regexError=""; else regexError.fix
end F"{ec} {regexError}" 2
