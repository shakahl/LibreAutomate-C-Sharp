 characters
int c='a' //without escape sequences
int c=C'\n' //with C escape sequences

 string constants
str s="text" //with QM escape sequences
str s=C"text" //with C escape sequences
str s=''text'' //without escape sequences
str s=''
multiline string
without escape sequences
not including newlines by ''
''

 strings with variables
str s=$"text {variable}" //with QM escape sequences
str s=$C"text {variable}" //with C escape sequences
str s=$''text {variable}'' //without escape sequences
str s=$''
multiline string
without escape sequences
with {variables}
''

   OR

 characters
int c=c'a' //without escape sequences
int c=C'\n' //with C escape sequences

 string constants
str s="text" //with QM escape sequences
str s=C"text" //with C escape sequences
str s='text' //without escape sequences, but '' is replaced to '
str s='
multiline string
without escape sequences
not including newlines by '
'

 strings with variables
str s=$"text {variable}" //with QM escape sequences
str s=$C"text {variable}" //with C escape sequences
str s=$'text {variable}' //without escape sequences, but '' is replaced to '
str s=$'
multiline string
without escape sequences
with {variables}
'


   OR, for compatibility with C#

 characters
int c='\n' //with C escape sequences

 string constants
str s="text" //with C escape sequences
str s=@"text" //without escape sequences
str s=q"text" //with QM escape sequences
str s=''text'' //without escape sequences
str s=''
multiline string
without escape sequences
not including newlines by ''
''

 strings with variables
str s=$"text {variable}" //with C escape sequences
str s=$@"text {variable}" //without escape sequences
str s=$q"text {variable}" //with QM escape sequences
str s=$''text {variable}'' //without escape sequences
str s=$''
multiline string
without escape sequences
with {variables}
''

 __________________

 Escape sequences could be:
 [-] - new line
 ['] - double quote
 [_] - tab
