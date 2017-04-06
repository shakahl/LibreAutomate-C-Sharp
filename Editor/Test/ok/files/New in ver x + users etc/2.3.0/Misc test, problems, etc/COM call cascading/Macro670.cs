out
Acc aa=acc("XPath HOME" "TEXT" win("XPath Syntax - Windows Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1801 0x40 0x20000040)

IAccessible a=aa.a
 out a.Parent.Name
out a.Parent.accChildCount ;;QM, IDispatch
out a.Parent.GetTypeInfoCount(_i) ;;QM, IDispatch
out _i


IDispatch d=a.Parent
 out d.accName(@)

 out d.accChildCount


ret
 d.GetTypeInfoCount(_i)
 out _i

ITypeInfo ti
if(d.GetTypeInfo(0 0 &ti)) ret

int i n
for i 0 100
	FUNCDESC* f
	ti.GetFuncDesc(i &f); err break
	 out "0x%X" f.memid

	ARRAY(BSTR) b.create(30)
	ti.GetNames(f.memid &b[0] b.len &n)
	b.redim(n)
	out "--- %i ---" i
	out b

	ti.ReleaseFuncDesc(f); f=0
