 Takes selected text from Dreamweaver editor and makes QM-colored code

DW_Css "None"

str s.getsel; err ret
str so

 s.replacerx("(?m)^" " ") ;;make comments

int ml=findc(s 10)>=0
int fl=4|8
if(ml) fl=2|4

QmCodeToHtml s &so 0 0 fl

key C`
if(ml) so+"[]"; DW_SelectSource s ;;select whole lines
so.setsel
key C`
