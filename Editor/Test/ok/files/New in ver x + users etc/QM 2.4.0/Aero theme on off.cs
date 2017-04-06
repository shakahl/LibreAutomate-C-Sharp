dll- "dwmapi"
	#DwmEnableComposition uCompositionAction
	#DwmIsCompositionEnabled *pfEnabled

 toggle Aero theme
if _winnt>=6 and !DwmIsCompositionEnabled(&_i)
	DwmEnableComposition !_i
