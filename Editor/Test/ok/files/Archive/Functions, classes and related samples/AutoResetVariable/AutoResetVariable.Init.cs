function int&variable [value]

 Remembers the address of a variable so that later could set it to 0 in destructor (when the function exits) or Reset().
 If value is not 0, sets variable=value.


_p=&variable
if(value) variable=value
