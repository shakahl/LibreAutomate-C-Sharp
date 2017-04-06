/exe
out
IXml x=CreateXml

 x.FromFile("$my qm$\test.xml")
 x.FromFile("q:\app\test.xml")
x.FromFile("q:\app\example.xml")
err out "Error: %s" x.XmlParsingError

XmlOut x 1

str s
x.ToString(s)
 out s

x.FromString(s)
x.ToString(s)
out s

 BEGIN PROJECT
 main_function  xml FromFile ToString
 exe_file  $my qm$\xml FromFile ToString.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {B4686BC2-66ED-4989-9709-EA405F0E92A2}
 END PROJECT
