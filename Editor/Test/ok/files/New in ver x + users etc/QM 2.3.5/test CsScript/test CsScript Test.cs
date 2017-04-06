/exe
 out

 out s

 PF
 IXml z._create
 z.FromString(s)
 IXmlNode n=z.RootElement.LastChild
 z.Delete(n)
 
 PN
 PO
 z.ToString(s)
 out s

 PF
 CsScript x.Init
 PN
 rep 5
	 _i=x.x.Test
	 PN
 PO
 out _i

CsScript x.Init
 rep(5) x.x.Test
PF
x.x.Test
PN; PO

 BEGIN PROJECT
 main_function  test CsScript Test
 exe_file  $my qm$\test CsScript Test.qmm
 flags  6
 guid  {5E78B728-C556-4AD8-9E18-7735D4DE5161}
 END PROJECT

 -1567281848
 -589445440
 speed: 703  95  88  89  94  94  
 1749148704
 speed: 700  104  87  86  87  99  
 -262139296
