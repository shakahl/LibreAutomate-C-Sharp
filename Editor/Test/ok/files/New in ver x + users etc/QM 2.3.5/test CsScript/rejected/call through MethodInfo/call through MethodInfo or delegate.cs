 Faster just 30-60%.
 Also tried fast invoke delegate, but could not make it work. Nevermind.

typelib MSCOREE "$windows$\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb"

str code=
 using System;
 public class Class1
 {
     static public int StaticFunc(int a)
     {
         return a*2;
     }
     public int Func(int a, int b)
     {
         return a+b;
     }
 }
int R

PF
CsScript x.AddCode(code)
PN

 rep 5
	 R=x.Call("StaticFunc" 1)
	 PN

ARRAY(VARIANT) a.create(1); a[0]=1
MSCOREE._MethodInfo mi
mi=x.x.GetMethodInfo("Class1.StaticFunc" a) ;;54
PN
rep 5
	R=mi.Invoke_3(0 a) ;;6-7
	 R=x.CallMI(mi 1) ;;11-12
	PN

  get delegate
 int fa=x.x.GetStaticMethod("Class1.StaticFunc" a) ;;error, cannot marshal
 PN
 out fa

PO
out R

#ret

 declarations and code:
	IDispatch'GetMethodInfo(BSTR'name SAFEARRAY*parameters)

        _MethodInfo GetMethodInfo(string name, object[] list);

        public _MethodInfo GetMethodInfo(string name, object[] list)
        {
            return m_asmhelper.GetMethodInfo(name, list);
        }

	#GetStaticMethod(BSTR'name SAFEARRAY*parameters) ;;it seems like cannot marshal ARRAY(VARIANT)
