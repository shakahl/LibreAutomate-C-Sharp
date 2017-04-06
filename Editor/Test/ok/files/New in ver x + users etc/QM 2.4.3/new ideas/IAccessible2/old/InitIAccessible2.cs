 /test IAccessible2

 To make IAccessible2 work, need IAccessible2Proxy.dll:
 	http://www.linuxfoundation.org/collaborate/workgroups/accessibility/iaccessible2/comproxydll
 Also need to place it in system folder and register as COM component.
 It seems that it's possible to use without registering:
 	http://lists.linux-foundation.org/pipermail/accessibility-ia2/2008-March/000452.html
 However also need to execute this code in FF process. Does not work if not.

interface# IAccessible2 :IUnknown
	Test
	{E89F726E-C4F4-4c19-bb19-b647d7fa8478}
def IID_IAccessible2 uuidof("{E89F726E-C4F4-4c19-bb19-b647d7fa8478}")

#ret
int+ __IA2Inited; if(__IA2Inited) ret

IUnknown u
dll- "$qm$\IAccessible2Proxy.dll" [DllGetClassObject]__IAccessible2Proxy_DllGetClassObject
if(__IAccessible2Proxy_DllGetClassObject(uuidof(IAccessible2) uuidof(IUnknown) &u)) end ERR_FAILED
 out u
int+ __IA2Cookie
if(!__IA2Cookie && CoRegisterClassObject(IID_IAccessible2 u CLSCTX_LOCAL_SERVER REGCLS_MULTIPLEUSE &__IA2Cookie)) end ERR_FAILED
 out __IA2Cookie
if(CoRegisterPSClsid(IID_IAccessible2,IID_IAccessible2)) end ERR_FAILED
__IA2Inited=1
 later will need to CoRevokeClassObject
