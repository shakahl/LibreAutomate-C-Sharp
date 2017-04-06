 QM
str csFile.expandpath("Q:\Test\test.cs")

PF
CsScript x.Init
PN
x.Load("$program files$\Mono\lib\mono\4.5\mcs.exe")
PN

 ARRAY(str) a="/?"
ARRAY(str) a.create(2)
a[0]="-out:Q:\Test\test.exe"
a[1]=csFile
x.Call("" a)

PN
PO


#ret
 Boo
csFile=str.ExpandPath("Q:\Test\test.cs")

PF
x as CsScript()
PN
x.Load("$program files$\Mono\lib\mono\4.5\mcs.exe")
PN

//a=("/?")
a=array(str, 2)
a[0]="-out:Q:\Test\test.exe"
a[1]=csFile
x.Call("", a)

PN
PO


#ret
//C#
string csFile=string.ExpandPath("Q:\Test\test.cs");

Perf.First();
CsScript x=new CsScript();
Perf.Next();
x.Load("$program files$\Mono\lib\mono\4.5\mcs.exe");
Perf.Next();

//var a=new string[1]{"/?"};
var a=new string[2];
a[0]="-out:Q:\Test\test.exe";
a[1]=csFile;
x.Call("", a);

Perf.Next();
Perf.Out();
