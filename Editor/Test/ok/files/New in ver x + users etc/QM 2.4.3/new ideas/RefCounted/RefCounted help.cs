 This macro will be displayed when you click this class name in code editor and press F1.
 Add class help here. Add code examples below EXAMPLES.

 EXAMPLES

#compile "__RefCounted"
RefCounted* x._new
 RefCounted x ;;need to prevent such usage. Also would need a another class that auto-releases a RefCounted*.

 x.AddRef
1
x.Release
 x.Release
