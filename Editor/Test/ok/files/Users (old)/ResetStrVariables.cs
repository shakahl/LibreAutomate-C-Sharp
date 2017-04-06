 /
function str&first num

 Clears specified number of str variables.
 No other variables must be declared between these.

 first - first variable
 num - number of variables

 EXAMPLE
 str a b
 str c d
 a="a"
 b="b"
 c="c"
 d="d"
  ...
 ResetStrVariables a 4


int i
str* p=&first
for(i 0 num) p[i].all
