 /
function# color

 Converts color format from 0xARGB to 0xBGR.


ret (color&0xff00) | (color&0xff<<16) | (color&0xff0000>>16)
