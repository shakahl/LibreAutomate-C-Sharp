 VARIANT v; int i=5; v=&i; int* p=+v; out *p ;;CastPointer
 Acc a=acc; Htm h.el=+a.a; IUnknown u=a.a ;;cast_interface_c
 Excel.Application u._setevents; u._getcontrol(0); u._create; u._getactive; u._getfile("")

 Excel.Application a.AddCustomList("")
 IAccessible c.Child(1)
 #opt dispatch 1
 Shell32.Shell h.MinimizeAll; h.Application.Release
 interface@ ITesttt :IDispatch Func()
 ITesttt t.Func

 ARRAY(str) a
 a.create(1); a.createlb(1 1); a.redim; a.insert(7); a.remove(7); a.lbound; a.ubound; a.len; a.ndim; a.sort; a.shuffle; a.lock; a.unlock
 a[]="test"; out a[0]

 SYSTEMTIME st; DATE d.add(st) ;;ffriend_c
 BSTR b.add("ff"); b.cmp("hh"); b.alloc(8); b.free; b.len

 VARIANT v(5) vv(2)
 v.add(vv); v.cmp(vv); v.div(vv); v.mul(vv); v.fix(vv); v.sub(vv) v.round

 dll- "dd" fuuuu
