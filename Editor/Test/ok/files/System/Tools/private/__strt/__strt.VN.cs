function$ [$default]

 Validates name for variable or other identifier, or sets default name if empty.

 this - (in out) name. The function corrects it if need (replaces invalid characters).
 default - default name to use if this is empty.
   Can be anything or empty. If this is empty, always sets this=default, even if default is empty.
   Can be 0 or other number. Then, if this empty, just sets this = the number.


if(!s.len) s=default
else s.replacerx("[^0-9A-Z_a-z]" "_"); if(isdigit(s[0])) s-"_"

ret s
