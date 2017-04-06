str s="
line1
line2
"

 is same as "line1[]line2"

 or

str s="/
line1
line2
/"

 here / can be any character except "

 F"string":

str s=F"
line1
line2
"

 _____________________

 But then difficult syntax highlighting.
 Eg what to do while user not finished typing the string.
 Maybe better use prefix:
str s=R"
line1
line2
"

 Then also can be inline:
str s=R"string without escape sequences"

 With F:
str s=FR"string without escape sequences"
str s=FR"
line1
line2
"

 This is very similar to D raw strings.

 _____________________

 Or:
str s=`
line1
line2
`

str s=`string without escape sequences`

str s=F`
line1
line2
`

str s=F`string without escape sequences`

 _____________________

 Or:
str s=
line1
line2
;

 But then problem: how to specify F? Maybe use another symbol?
str s=+
line1
line2
;

 Syntax highlighting:
 When user types like 'str s=Enter', automatically insert another newline and ;:
str s=
now caret is here, and user can type string immediately
;

 String-highlight all lines until next ; line. If there is no such line, don't highlight.

 To escape ;:
str s=\\  ;;any test that begins with \
line1
line2
\\

 or

str s=+
line1
{";"}
line2
;

 or
str s=
line1
/semicolon/
line2
;
s.findreplace("/semicolon/" ";")
