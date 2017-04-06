function [int&x] [int&y] [int&width] [int&height]

 Gets position (in screen) and size.

 x, y, width, height - variables for position and size. Can be 0.


if(!a) end ERR_INIT
if(!&x) &x=&_i
if(!&y) &y=&_i
if(!&width) &width=&_i
if(!&height) &height=&_i
a.Location(&x &y &width &height elem); err end _error
