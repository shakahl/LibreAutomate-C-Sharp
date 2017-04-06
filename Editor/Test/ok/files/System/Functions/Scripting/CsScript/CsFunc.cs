 /
function` $code [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]

 Compiles C# script and calls its first public static function.
 Returns its return value.

 code - the script.
   Can be string, file or macro (like "macro:MacroName"), like with <help>CsScript.AddCode</help>.
   If "", gets caller's text that follows #ret line.
   The script can contain:
     Normal C# code with classes etc. Will be called the first found public static function of the first found public class.
     Single function without a class. Can be preceded by 'using' lines (don't need 'using System;') and comments. May be without 'public' and 'static'. The code must not contain word "class", even in comments or strings.
 a1-a10 - arguments.

 REMARKS
 Automatically converts argument and return types, if possible. To pass by reference, instead <help "CsScript example - create object">create object</help>.

 See also: <CsScript.Call>, <CsExec>, <CsScript help>.

 Errors: <.>

 EXAMPLES
 str classlessFunction=
  int Add(int a, int b) { return a+b; }
 int R=CsFunc(classlessFunction 2 3)
 out R
 
 str normalCode=
  using System;
  public class Test
  {
    public static int Add(int a, int b)
    {
      _Out(a + " + " + b);
      return a+b;
    }
    static void _Out(object x) { Console.WriteLine(x.ToString()); }
  }
 out CsFunc(normalCode 20 30)


#exe addtextof "<script>"
opt noerrorshere 1
opt nowarningshere 1

CsScript x.AddCode(code 0x10000)

RECT r.bottom=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE|FADF_AUTO|FADF_FIXEDSIZE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1
ret x.x.Call("*" &sa)
