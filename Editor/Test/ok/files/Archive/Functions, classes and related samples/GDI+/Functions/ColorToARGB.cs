 /
function# color [alpha] ;;alpha: 255 opaque (default), 0 transparent

 Converts color format 0xBBGGRR (COLORREF) to 0xAARRGGBB or vice versa.
 alpha - transparency. Must be between 0 (transparent) and 255 (opaque). If omitted, uses 255.


if(getopt(nargs)<2) alpha=255
byte* p=&color; _i=p[0]; p[0]=p[2]; p[2]=_i; p[3]=alpha
ret color
