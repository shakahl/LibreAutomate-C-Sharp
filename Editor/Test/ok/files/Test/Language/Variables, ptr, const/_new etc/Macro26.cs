typelib Excel {00020813-0000-0000-C000-000000000046} 1.2
#opt dispatch 1 ;;call functions through IDispatch::Invoke (may not work otherwise)
Excel.Application* p;;._new ;;syntax
p._new(3)
p[0]._create
p[0].Visible=TRUE
p[1]._create
p[1].Visible=TRUE
p[2]._create
p[2].Visible=TRUE
2
 p[0].Release
p._delete
4
