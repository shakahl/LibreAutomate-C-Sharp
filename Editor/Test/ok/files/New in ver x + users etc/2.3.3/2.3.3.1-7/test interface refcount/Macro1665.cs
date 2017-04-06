out

IAccessible a
 IDispatch a
int w=win("QM TOOLBAR" "QM_toolbar")
AccessibleObjectFromWindow(w OBJID_CLIENT IID_IAccessible &a)

outref a

 Function201 a

 IDispatch d=a
 Function201 d
 d=0

 VARIANT d=a
 Function201 d
 d=0

outref a
 rep 3
	 Function201 a
	 outref a

 Function202 a
 outref a
