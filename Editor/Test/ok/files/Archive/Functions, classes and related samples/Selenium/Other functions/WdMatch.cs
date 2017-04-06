function! BSTR's $regExp

 Returns 1 if string s matches regular expression regExp. Else 0.
 Used instead of C# Regex.IsMatch.

_s=s
ret findrx(_s regExp)>=0
