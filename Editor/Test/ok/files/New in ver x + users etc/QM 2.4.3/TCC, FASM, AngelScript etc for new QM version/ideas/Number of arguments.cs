If a function has optional or ... parameters, add a hidden parameter ____nArgs____.
Else either don't allow to use opt(nargs) or add it into C code as a constant.
For callback functions, either don't allow optional/... parameters, or let opt(nargs) always return the number of all parameters (add to C code as constant).
