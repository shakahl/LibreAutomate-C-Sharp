 /
function!

dll- "$qm$\IAccessible2Proxy.dll" [DllGetClassObject]__ia2_DllGetClassObject GUID*clsid GUID*iid IUnknown*pOut
typelib IA2 "$qm$\IA2.tlb"
def IID_IAccessible2 uuidof(IA2.IAccessible2)

int+ g_ia2Cookie
IUnknown u
int hr
 hr=__ia2_DllGetClassObject(IID_IAccessible2 IID_IUnknown &u)
hr=__ia2_DllGetClassObject(IID_IAccessible2 uuidof(IUnknown) &u)
if(hr) out _s.dllerror("DllGetClassObject" "" hr); ret
hr=CoRegisterClassObject(IID_IAccessible2 u CLSCTX_INPROC_SERVER REGCLS_MULTIPLEUSE &g_ia2Cookie)
if(hr) out _s.dllerror("CoRegisterClassObject" "" hr); ret
u=0
CoRegisterPSClsid(IID_IAccessible2 IID_IAccessible2)
ret 1

 ARRAY(GUID*) g.create(14)
 g[0]=uuidof(IA2.IAccessible2)
 g[1]=uuidof(IA2.IAccessibleAction)
 g[2]=uuidof(IA2.IAccessibleApplication)
 g[3]=uuidof(IA2.IAccessibleComponent)
 g[4]=uuidof(IA2.IAccessibleEditableText)
 g[5]=uuidof(IA2.IAccessibleHyperlink)
 g[6]=uuidof(IA2.IAccessibleHypertext)
 g[7]=uuidof(IA2.IAccessibleImage)
 g[8]=uuidof(IA2.IAccessibleRelation)
 g[9]=uuidof(IA2.IAccessibleTable)
 g[10]=uuidof(IA2.IAccessibleTable2)
 g[11]=uuidof(IA2.IAccessibleTableCell)
 g[12]=uuidof(IA2.IAccessibleText)
 g[13]=uuidof(IA2.IAccessibleValue)

 ARRAY(GUID*) g.create(18)
 g[0]=uuidof(IA2.IAccessible2)
 g[1]=uuidof(IA2.IAccessibleAction)
 g[2]=uuidof(IA2.IAccessibleApplication)
 g[3]=uuidof(IA2.IAccessibleComponent)
 g[4]=uuidof(IA2.IAccessibleEditableText)
 g[5]=uuidof(IA2.IAccessibleHyperlink)
 g[6]=uuidof(IA2.IAccessibleHypertext)
 g[7]=uuidof(IA2.IAccessibleImage)
 g[8]=uuidof(IA2.IAccessibleRelation)
 g[9]=uuidof(IA2.IAccessibleTable)
 g[10]=uuidof(IA2.IAccessibleTable2)
 g[11]=uuidof(IA2.IAccessibleTableCell)
 g[12]=uuidof(IA2.IAccessibleText)
 g[13]=uuidof(IA2.IAccessibleValue)
 g[14]=uuidof(IA2.IAccessible2_2)
 g[15]=uuidof(IA2.IAccessibleDocument)
 g[16]=uuidof(IA2.IAccessibleHypertext2)
 g[17]=uuidof(IA2.IAccessibleText2)
 int i; for(i 0 g.len) out CoRegisterPSClsid(g[i] IID_IAccessible2)

ARRAY(GUID) g.create(18)
g[0]=*uuidof(IA2.IAccessible2)
g[1]=*uuidof(IA2.IAccessibleAction)
g[2]=*uuidof(IA2.IAccessibleApplication)
g[3]=*uuidof(IA2.IAccessibleComponent)
g[4]=*uuidof(IA2.IAccessibleEditableText)
g[5]=*uuidof(IA2.IAccessibleHyperlink)
g[6]=*uuidof(IA2.IAccessibleHypertext)
g[7]=*uuidof(IA2.IAccessibleImage)
g[8]=*uuidof(IA2.IAccessibleRelation)
g[9]=*uuidof(IA2.IAccessibleTable)
g[10]=*uuidof(IA2.IAccessibleTable2)
g[11]=*uuidof(IA2.IAccessibleTableCell)
g[12]=*uuidof(IA2.IAccessibleText)
g[13]=*uuidof(IA2.IAccessibleValue)
g[14]=*uuidof(IA2.IAccessible2_2)
g[15]=*uuidof(IA2.IAccessibleDocument)
g[16]=*uuidof(IA2.IAccessibleHypertext2)
g[17]=*uuidof(IA2.IAccessibleText2)
 int i; for(i 0 g.len) out CoRegisterPSClsid(&g[i] IID_IAccessible2)
int i
for(i 0 g.len)
	 out _s.FromGUID(g[i])
	 GUID _g; CoGetPSClsid(&g[i] &_g)
	CoRegisterPSClsid(&g[i] IID_IAccessible2)
ret 1
