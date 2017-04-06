/exe

 mes 1
str code=
 using System;
 using System.Windows.Forms;
 
 public class Test
 {
     static public void Main ()
     {
         Console.WriteLine("ha");
 		MessageBox.Show($"bb {77} bb");
     }
 }
str sCS.expandpath("$temp qm$\test.cs")
code.setfile(sCS)
str sDll.expandpath("$temp qm$\test.exe")
 str sDll.expandpath("$temp qm$\test.dll")
del- sDll; err

 str csc="C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\csc.exe" ;;75 ms, C# 5
 str csc="$program files$\MSBuild\14.0\Bin\csc.exe" ;;350 ms, C# 6
str csc="Q:\Test\Csc\csc.exe" ;;350 ms, C# 6
 str csc="$program files$\MSBuild\14.0\Bin\MSBuild.exe"
 SetCurDir "Q:\app\comp"; str csc="Q:\app\comp\csc.exe" ;;to avoid conhost process, tried to copy csc.exe to another folder and patch to make GUI. It succeeds, but csc.exe runs cvtres.exe (console) from .NET folder anyway. It is protected, cannot rename etc, even by a System process. Slower.
str refer="C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Windows.Forms.dll"
 str options="/noconfig /unsafe /utf8output /fullpaths /target:library /nologo /optimize /platform:x86 /nowin32manifest"
 str options="/noconfig /unsafe /utf8output /fullpaths /target:library /nologo /optimize /platform:anycpu"
str options="/noconfig /unsafe /utf8output /fullpaths /target:exe /nologo /optimize /platform:anycpu"
str cl=F"''{csc}'' {options} /r:{refer} /out:''{sDll}'' ''{sCS}''"
 cl=F"''{csc}'' /?"

WakeCPU
rep 1
	PF
	int R=RunConsole1(cl _s)
	 int R=CreateProcessSimple1(cl 1|8)
	 _spawnl _P_WAIT F"{csc}" F"{options} /r:{refer} /out:''{sDll}'' ''{sCS}''" 0
	PN
	PO
	 if(R) out "csc returned: %i" R
	if(_s.len) out _s
	 if(R) ret
ret

 speed: default 170 ms, with /noconfig 70-80 ms

PF
CsScript x.Init
PN
x.Load(sDll)
PN
x.Call("Test.Main")
PN
PO

 BEGIN PROJECT
 main_function  run csc
 exe_file  $my qm$\run csc.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  70
 guid  {C3CC4B11-F4DF-44D2-A572-C476FD7E0096}
 END PROJECT
