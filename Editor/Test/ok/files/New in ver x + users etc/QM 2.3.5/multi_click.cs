spe 20 ;;set speed, ie number of ms to wait after lef
rep
	int+ g_multi_click; if(!g_multi_click) break ;;this will stop the macro on left button up. Don't remove.
	 out "click" ;;debug
	lef

 note: this macro must be named "multi_click". If want to rename, also change macro name in "mouse_left_macro_on_off".
