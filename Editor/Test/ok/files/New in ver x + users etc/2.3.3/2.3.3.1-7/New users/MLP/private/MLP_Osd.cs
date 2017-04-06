 /
function what $text timeout [flags] ;;what: 0 MSB, 1 LSB, 2 PRG.  flags: 1 don't change text.  If timeout -1, no timeout.

 This function shows OSD.
 Edit it if need to change OSD position, color or font size.
 __________________________________

 Change these values.

int color x y(25) fontSize(150)
sel what
	case 0 x=1; color=0x0000FF
	case 1 x=350; color=0x00FF00
	case 2 x=700; color=0x00FFFF
 __________________________________

if(flags&1) text=g_mlp.text[what]; else g_mlp.text[what]=text
int f=16|0x100; if(timeout<0) f|8
OnScreenDisplay text timeout x y "LSD" fontSize color f F"MLP_{what}"
