 /lock {int+ g_editorontop=0}
check :g_editorontop^1; SendMessage id(9999 TriggerWindow) TB_CHECKBUTTON 1 g_editorontop * function.ico
 g_editorontop is a global variable. It is 1 when the toolbar button is checked, 0 when unchecked. You can use it in other macros.
 In SendMessage, 1 is 0-based line index of the button in toolbar text.
