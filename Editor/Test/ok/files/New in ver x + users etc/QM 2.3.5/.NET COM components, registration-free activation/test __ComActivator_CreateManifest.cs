out
#if 0
__ComActivator_CreateManifest "ARServicesMgr.dll"
 __ComActivator_CreateManifest "ARServicesMgr.dll[]MailBee.dll"

 out _s.FromGUID(uuidof(Services.clsServices))

__ComActivator ca.Activate("ARServicesMgr.X.manifest")
Services.clsServices ss._create
out ss

#else

 __ComActivator_CreateManifest "QmNet.dll" 1
__ComActivator_CreateManifest "qmcs.dll" 1|2

__ComActivator ca.Activate("qmcs.X.manifest")
 __ComActivator ca.Activate("QmNet.dll,2")
IDispatch d._create("qmcs.Host")
 IDispatch d._create("{EA25C876-028E-4E58-A12A-F8A8143DF658}")
out d

 __ComActivator ca.Activate("QmNet.X.manifest")
 typelib TypelibName "$qm$\QmNet.tlb"
 TypelibName.CsScript x._create
 out x
