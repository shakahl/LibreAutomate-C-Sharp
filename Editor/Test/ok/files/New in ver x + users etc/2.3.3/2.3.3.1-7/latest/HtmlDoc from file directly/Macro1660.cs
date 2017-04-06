/exe
out
 str s.getfile("mk:@MSITStore:Q:\app\QM2Help.chm::/QM_Help/IDH_INTERFACE.html")
 out s

HtmlDoc d

d.SetOptions(2)
d.InitFromWeb("http://www.quickmacros.com/test/test.html")

 d.InitFromFile("mk:@MSITStore:Q:\app\QM2Help.chm::/QM_Help/IDH_INTERFACE.html")
 d.InitFromFile("q:\app\htmlhelp/QM_Help/IDH_INTERFACE.html" 1)
 d.InitFromWeb("http://www.quickmacros.com")
 d.InitFromFile("no file.html")

str s=d.GetHtml
s.fix(2000)
out s
 out d.GetText

 BEGIN PROJECT
 main_function  Macro1660
 exe_file  $my qm$\Macro1660.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {B65398D5-40A1-4B2C-98F9-92721337E766}
 END PROJECT
