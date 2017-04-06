function~ $name

 Gets value of an attribute.
 Returns empty string if fails or if the attribute does not exist.

 name - attribute name (eg "href").


if(!node) end ERR_INIT
BSTR b1(name) b2; word w
node.get_attributesForNames(1 b1 w b2); err ret
ret b2
