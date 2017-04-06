 /
function# $number str&_word

 number - string that begins with character '0' - '9'.
 _word - str variable that receives word "One" - "Nine".

_word.fix(0)
if(!number) ret
int i=number[0]-'0'
if(i<0 or i>9) ret
_word.getl("Zero[]One[]Two[]Three[]Four[]Five[]Six[]Seven[]Eith[]Nine" i)
ret 1
