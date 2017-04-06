/exe 1

out
PF
Acc ap.FromWindow(GetDesktopWindow OBJID_CLIENT)
out ap.ChildCount
ARRAY(Acc) a
ap.GetChildObjects(a 0 "" "" "" 16|32)
out a.len
int i
for i 0 a.len
	 str role name
	 a[i].Role(role)
	 name=a[i].Name
	 out F"{role} {name}"
	int w=child(a[i])
	 outw2 w
	if GetWinStyle(w 1)&WS_EX_NOREDIRECTIONBITMAP
		outw2 w
PN
PO

 BEGIN PROJECT
 main_function  test get windows with AccessibleChildren
 exe_file  $my qm$\test get windows with AccessibleChildren.qmm
 flags  6
 guid  {3854EC20-41DC-48C5-91B7-F22FB0E48FEC}
 END PROJECT
