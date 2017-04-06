typelib mscorlib {BED7F4EA-1A96-11D2-8F08-00A0C9A6186D} 2.0

IDispatch s._create(uuidof(mscorlib.Stack))
s.Push("test")
out s.Pop

 IDispatch d._create(uuidof(mscorlib.Directory)) ;;fails
 IDispatch d._create(uuidof(mscorlib.Object))
 out d.Directory.Exists("q:\app")

 IDispatch s._create(uuidof(mscorlib.String))
