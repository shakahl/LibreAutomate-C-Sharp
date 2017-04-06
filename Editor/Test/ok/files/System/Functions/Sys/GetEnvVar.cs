 /
function$ $name str&value

 Gets an environment variable.
 Returns value if successful, 0 if failed.

 value - variable for value.

 Added in: QM 2.3.0.


int n1(4) n2
 g1
BSTR b.alloc(n1)
n2=GetEnvironmentVariableW(@name b n1)
if(n2>n1) n1=n2; goto g1
if(n2) value.ansi(b); ret value
value.all
