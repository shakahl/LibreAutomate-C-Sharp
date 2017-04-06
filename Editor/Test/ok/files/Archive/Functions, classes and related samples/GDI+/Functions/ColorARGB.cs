 /
function# [!red] [!green] [!blue] [!alpha] ;;alpha: 255 opaque (default), 0 transparent

 Returns color value in format 0xAARRGGBB.

 red, green, blue - color components. Must be between 0 and 255.
 alpha - transparency. Must be between 0 (transparent) and 255 (opaque). If omitted, uses 255.


if(getopt(nargs)<4) alpha=255
ret blue | (green<<8) | (red<<16) | (alpha<<24)
