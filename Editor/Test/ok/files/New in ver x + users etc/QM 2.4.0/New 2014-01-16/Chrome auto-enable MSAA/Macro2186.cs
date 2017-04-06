InitIAccessible2
int w=win("- Google Chrome" "Chrome_WidgetWin_1")
Acc a.FromWindow(w OBJID_CLIENT)

IUnknown u
IServiceProvider sp=+a.a
sp.QueryService(IID_IAccessible IID_IAccessible2 &u)
out u
