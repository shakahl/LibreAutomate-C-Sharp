 InitIAccessible2
 #ret
typelib IA2 "$qm$\IA2.tlb"
def IID_IAccessible2 uuidof(IA2.IAccessible2)

int w=win("Firefox" "Mozilla*WindowClass" "" 0x4)
 Acc a.FromWindow(w OBJID_CLIENT)
Acc a.FindFF(w "#document" "" "" 0x1000 3)
 out a.a
a.Role(_s); out _s

IUnknown u
 u=__QueryService(a.a IID_IAccessible2 IID_IAccessible2)
 u=__QueryService(a.a IID_IAccessible2 IID_IAccessible)

IServiceProvider sp=+a.a
sp.QueryService(IID_IAccessible IID_IAccessible2 &u)


out u ;;0

 interface# IAccessible2 :IUnknown
	 Test
	 {E89F726E-C4F4-4c19-bb19-b647d7fa8478}
 
 IAccessible2 a2=+a.a
 out a2
