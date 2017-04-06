 /
function` $code [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]

 Compiles VB.NET script and calls its first public shared function.
 Returns its return value.

 code - the script.
   Can be string, file or macro (like "macro:MacroName"), like with <help>CsScript.AddCode</help>.
   If "", gets caller's text that follows #ret line.
   The script must contain normal VB.NET code with classes etc. Will be called the first found public shared function of the first found public class or module.
 a1-a10 - arguments.

 REMARKS
 Automatically converts argument and return types, if possible. To pass by reference, instead <help "CsScript example - create object">create object</help>.

 See also: <VbExec>, <CsScript.Call>, <CsScript help>.

 Errors: <.>

 EXAMPLES
  example with code in string and using Module
 str code=
  Imports System
  Public Module Test
  Function Add(a as Integer, b as Integer) As Integer
   Return a+b
  End Function
  End Module
 out VbFunc(code 5 2)

  example with code in same macro and using Class
 out VbFunc("" 5 2)
 #ret
 Imports System
 Public Class Test
 Public Shared Function Add(a as Integer, b as Integer) As Integer
  Return a+b
 End Function
 End Class


#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1

CsScript x.SetOptions("language=VB")
x.AddCode(code 0x10000)

RECT r.bottom=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE|FADF_AUTO|FADF_FIXEDSIZE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1
ret x.x.Call("*" &sa)
