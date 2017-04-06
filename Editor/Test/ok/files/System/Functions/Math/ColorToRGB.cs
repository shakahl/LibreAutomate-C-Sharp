 /
function color int&red int&green int&blue

 Gets color components.

 color - color value in format 0xBBGGRR.
 red, green and blue - variables that receive red, green and blue components (0 to 255).


red=color&255
green=color>>8&255
blue=color>>16&255
