 /exe

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "macro:Macro2032.bmp" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan ":10 test auto add macro to exe.bmp" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline

 AddTrayIcon "mouse.ico" "tt" "tray_onclick" "tray_onrclick"
 Tray t.AddIcon("mouse.ico" "tt" 0 0 0 "tray_onclick" "tray_onrclick")
 10

 CsExec ""
 CsExec "macro:test auto add macro to exe"
 CsFunc ""
 CsScript x.AddCode(""); x.Call("Main")
 CsScript x.Exec("")
 CsScript x.Compile("" "$temp$\o.dll"); x.Load("$temp$\o.dll"); x.Call("Main")

str s=
 macro:Dialog124
 macro:Dialog125
 invalid
 macro:tray_onclick
 

s=
 :3 mouse.ico
 &:2 function.ico
 invalid
 :4 paste.ico

 BEGIN PROJECT
 main_function  test auto add macro to exe
 exe_file  $my qm$\Macro2032.qmm
 flags  23
 guid  {7D223258-8F78-4294-8672-4CD2C0593EA4}
 END PROJECT

#ret
using System;
public class Kkk
{
public static void Main()
{
Console.Write("Main");
}
}