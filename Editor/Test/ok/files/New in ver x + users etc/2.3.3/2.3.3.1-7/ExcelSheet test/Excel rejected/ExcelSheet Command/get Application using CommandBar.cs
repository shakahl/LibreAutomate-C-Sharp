 Does not work as documented.
 Gets just IAccessible, not CommandBar.

out
typelib Office {2DF8D04C-5BFA-101B-BDE5-00AA0044DE52} 2.3

int w=win("Microsoft Excel" "XLMAIN")
 int w=win("Microsoft Visual" "wndclass_desked_gsk")
 int w=win("Microsoft Outlook" "rctrl_renwnd32")

ARRAY(int) a
child("" "MsoCommandBar" w 0 0 0 a)

int i c
for i 0 a.len
	c=a[i]
	outw c
	
	 Microsoft_VisualStudio_CommandBars.CommandBar cb
	IAccessible cb=0
	if(AccessibleObjectFromWindow(c OBJID_NATIVEOM uuidof(cb) &cb)) continue
	out cb
	
	 int** pp=+cb
	 int* p=pp[0]
	 int j
	 for(j 0 40) out p[j] ;;30 functions. If IAccessible 21. In CommandBar more.
	
	 Office.CommandBar x.
	 Office.CommandBar cb=0
	 if(AccessibleObjectFromWindow(c OBJID_NATIVEOM uuidof(cb) &cb)) continue
	 out cb
	
	 IDispatch d=0
	 if(AccessibleObjectFromWindow(c OBJID_NATIVEOM uuidof(d) &d)) continue
	 out d
	
	 d=d.Application
	 out d
	
	
	
	 out d.Name
	 out d.get_Name
	
	 memcpy &cb &d 4; memset &d 0 4
	 IDispatch ap=cb.Application ;;exception
	 out ap
	
	 cb=d ;;error no interface
	 IAccessible ac=d
	 out ac.Name
	
	 cb=__QueryService(ac uuidof(cb))
	 out cb
	
