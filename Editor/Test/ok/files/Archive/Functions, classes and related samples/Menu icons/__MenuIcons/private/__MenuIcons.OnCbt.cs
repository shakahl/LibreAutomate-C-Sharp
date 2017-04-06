function# code wParam lParam

if(code=HCBT_CREATEWND and wintest(wParam "" "#32768"))
	SetProp wParam "__mi_sub" SubclassWindow(wParam &__MenuIcons_Subclass)

ret CallNextHookEx(0 code wParam +lParam)
