;Syntax
;str gettok(string number [n] [delim] [flags])


;Parts
;   string - input string
;   number - Number of the token to return
;   n - max. number of tokens required. Default: -1. If n is negative, function tokenizes whole string.
;       If n is less than -1, function uses its absolute value as a hint to allocate array.
;   delim - delimiter characters. String or literal 0 or 1. Default: 0. If delim is 0, all characters
;           except alphanumeric and underscore are delimiters. If delim is 1, delimiters are blanks,
;           quotes and parentheses.
;   flags - combination of values listed below. Default: 0.
;
;     1 substitute first delimiter character after each token to 0 ( string must not be constant);
;     2 as n-1 th token get all right part;
;     4 phrase in " " is single token;
;     8 phrase in ( ) is single token;
;     16 phrase in [ ] is single token;
;     32 phrase in { } is single token;
;     64 phrase in < > is single token;
;     128 phrase in ' ' is single token;
;     256 delim is table of delimiters.

; Returns the 'number'th token of the string.
; If n'th token doesn't exist, launch error

function~ ~input #number [#n] [~delim] [#flags]
if(!delim) delim=0
if(!n) n=-1
if(!flags) flags=0
ARRAY(str) s
tok(input s n delim flags)
if (s.len>number)
	ret s[number]
else
	end "invalid argument"