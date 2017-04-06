function str&styleStr style [exStyle]

int s(style) e(exStyle)
str& k=styleStr
k.fix(0)

if(s&WS_POPUP) k+"popup,"
if(s&WS_CHILD) k+"child,"
if(s&WS_MINIMIZE) k+"minimize,"
if(s&WS_VISIBLE) k+"visible,"
if(s&WS_DISABLED) k+"disabled,"
if(s&WS_CLIPSIBLINGS) k+"clipsiblings,"
if(s&WS_CLIPCHILDREN) k+"clipchildren,"
if(s&WS_MAXIMIZE) k+"maximize,"
if(s&WS_CAPTION=WS_CAPTION) k+"caption,"
else if(s&WS_BORDER) k+"border,"
else if(s&WS_DLGFRAME) k+"dlgframe,"
if(s&WS_VSCROLL) k+"vscroll,"
if(s&WS_HSCROLL) k+"hscroll,"
if(s&WS_SYSMENU) k+"sysmenu,"
if(s&WS_THICKFRAME) k+"thickframe,"
if s&WS_CHILD
	if(s&WS_GROUP) k+"group,"
	if(s&WS_TABSTOP) k+"tabstop,"
else
	if(s&WS_MINIMIZEBOX) k+"minimizebox,"
	if(s&WS_MAXIMIZEBOX) k+"maximizebox,"

if(e&WS_EX_DLGMODALFRAME) k+"ex_dlgmodalframe,"
if(e&WS_EX_NOPARENTNOTIFY) k+"ex_noparentnotify,"
if(e&WS_EX_TOPMOST) k+"ex_topmost,"
if(e&WS_EX_ACCEPTFILES) k+"ex_acceptfiles,"
if(e&WS_EX_TRANSPARENT) k+"ex_transparent,"
if(e&WS_EX_MDICHILD) k+"ex_mdichild,"
if(e&WS_EX_TOOLWINDOW) k+"ex_toolwindow,"
if(e&WS_EX_WINDOWEDGE) k+"ex_windowedge,"
if(e&WS_EX_CLIENTEDGE) k+"ex_clientedge,"
if(e&WS_EX_CONTEXTHELP) k+"ex_contexthelp,"
if(e&WS_EX_RIGHT) k+"ex_right,"
if(e&WS_EX_LEFT) k+"ex_left,"
if(e&WS_EX_RTLREADING) k+"ex_rtlreading,"
if(e&WS_EX_LTRREADING) k+"ex_ltrreading,"
if(e&WS_EX_LEFTSCROLLBAR) k+"ex_leftscrollbar,"
if(e&WS_EX_RIGHTSCROLLBAR) k+"ex_rightscrollbar,"
if(e&WS_EX_CONTROLPARENT) k+"ex_controlparent,"
if(e&WS_EX_STATICEDGE) k+"ex_staticedge,"
if(e&WS_EX_APPWINDOW) k+"ex_appwindow,"
if(e&WS_EX_LAYERED) k+"ex_layered,"
if(e&WS_EX_NOINHERITLAYOUT) k+"ex_noinheritlayout,"
if(e&WS_EX_LAYOUTRTL) k+"ex_layoutrtl,"
if(e&WS_EX_COMPOSITED) k+"ex_composited,"
if(e&WS_EX_NOACTIVATE) k+"ex_noactivate,"

styleStr.rtrim(",")
