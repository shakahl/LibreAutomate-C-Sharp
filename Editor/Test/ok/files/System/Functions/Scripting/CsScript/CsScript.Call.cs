function` $name [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]

 Calls a public static function (method or property).
 Returns its return value.

 name - function name.
   Can be full name, like "Class1.Func1", "Namespace1.Class1.Func1", "VbModule.Func1", or just function name.
   If function name is "*", calls the first found public static function.
   If function name is "", calls the entry function (usually Main; see <open>CsScript.Exec</open>).
   To set or get a property, add set_ or get_ prefix to the property name.
 a1-a10 - arguments.

 REMARKS
 Use <help>CsScript.AddCode</help> to add code that contains the function.
 The class or VB module must be public too.
 Automatically converts argument and return types, if possible. For example, int to bool, int to uint, int to string, string to DateTime, DATE to DateTime. To pass by reference, instead <help "CsScript example - create object">create object</help>.
 Supports overloads that have different number of parameters. If number of parameters is same, calls the first overload and converts argument types if need.

 See also: <CsFunc>, <VbFunc>

 EXAMPLE
 //QM code
 CsScript x.AddCode("") ;;if "", gets text that follows #ret line in this macro
 int M=x.Call("Test.Add" 10 5)
 x.Call("Test.set_Prop" 100)
 int P=x.Call("Test.get_Prop")
 out M
 out P
 #ret
 //C# code
 using System;
 public class Test
 {
 public static int Add(int a, int b) { return a+b; } //method
 public static int Prop { get; set; } //property
 }


opt noerrorshere 1

if(!x) end ERR_INIT

RECT r.bottom=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE|FADF_AUTO|FADF_FIXEDSIZE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1
ret x.Call(name &sa)
