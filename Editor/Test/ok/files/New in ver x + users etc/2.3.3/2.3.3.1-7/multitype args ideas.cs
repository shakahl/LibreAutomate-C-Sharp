Ideas how to support multitype arguments of UDF.

_________________________________________

QM defines a special type:
type VARARG vt

User defines type that is inherited from VARARG and has members of types supported by the function.
type va_int_str :VARARG int'i str's

When calling the function, QM would set vt to the index of the arg (1 for i, 2 for s).

_________________________________________

Allow functions in functions:
function TYPE1'a
...
_
function F2 TYPE2'b
...

Then can call:
Function a
Function.F2 b
