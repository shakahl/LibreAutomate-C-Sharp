function# str&winVar [$_default] [str&winFind] [str&sComments]

 Parses window/control selector control variable (this).
 Returns type: 0 none/screen, 1 window, 2 control.

 winVar - receives handle or an expression that returns handle, usually win(), child(...) or id(...).
 _default - default winVar if screen/none.
 winFind - receives "int var=win(...)[]", or empty.
 sComments - receives comments.

ARRAY(str) a=s
if a.len!4
	winVar=_default
	if(&winFind) winFind.all
	if(&sComments) sComments.all
	ret
winVar=a[1]
if(&winFind) winFind=a[2]; if(winFind.len) winFind+"[]"
if(&sComments) sComments=a[3]
s.flags|128
ret val(a[0])
