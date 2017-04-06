 /
function# $findstr $replacestr [hwnd] [whole] [insens] [regexpr] [justtest]

 Finds/replaces in window text or selected text.
 Returns the number of replacements.

 findstr - string to find.
 replacestr - replaces with this string.
 hwnd - if 0, uses selected text, else uses text of window or child window whose handle is hwnd.
 whole - if nonzero, finds whole words.
 insens - if nonzero, case insensitive.
 regexpr - if nonzero, findstr is regular expression. See <help>str.replacerx</help>.
 justtest - if nonzero, does not replace but just returns the number of found substrings.

 EXAMPLES
  Replace all "Mart" with "April" in selected text. Find whole words, case insensitive:
 Replace "Mart" "April" 0 1 1
 
  Replace all "Untitled - Notepad" with "Notepad" in Notepad:
 lpstr s1="Untitled - Notepad"
 lpstr s2="Notepad"
 Replace s1 s2 id(15 "Notepad")


if(empty(findstr)) ret

opt waitmsg -1
str s
if(hwnd) s.getwintext(hwnd) else s.getsel
if(!s.len and !hwnd) 0.5; s.getsel
if(!s.len) ret
int flags nfound
if(insens) flags|1
if(whole) flags|2
if(regexpr)
	nfound=s.replacerx(findstr replacestr flags)
	err int- t_hDlg; sub_sys.MsgBox t_hDlg _error.description "Regular expression error" "i"; ret
else nfound=s.findreplace(findstr replacestr flags)
if(nfound and !justtest)
	if(hwnd) s.setwintext(hwnd) else s.setsel
ret nfound
