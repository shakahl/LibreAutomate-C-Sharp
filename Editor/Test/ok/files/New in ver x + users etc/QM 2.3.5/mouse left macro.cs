 This is the main macro. Edit it to do what you need.

spe 20 ;;set lef speed
rep 14
	 ifk-((1)) break ;;end loop if left button released. However this does not work if this macro clicks.
	out "click" ;;debug
	lef

 Note: the trigger eats mouse left button when global variable g_mouse_left_macro is nonzero.
 You can use Ctrl+Shift+Alt+D to temporarily disable QM triggers and enable again.
