 CsScript x.AddCode("using System;class A{ static void Main() { Console.Write(''test''); }}")
CsScript x.AddCode("using System;class A{ static void Main(string[ ] a) { Console.Write(''test''); }}")
 x.Call("")
 x.Call("" "")

ARRAY(BSTR) a.create(1); a[0]="/tt"
 ARRAY(BSTR) a.create(0)
 x.Call("" a 5)
 x.x.Call("" a) ;;error, must be in VARIANT


x.Call("" a)
