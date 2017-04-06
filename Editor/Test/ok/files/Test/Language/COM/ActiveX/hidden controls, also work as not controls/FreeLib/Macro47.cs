 typelib FreeLibSrc {B6C1EA38-375B-11D4-93AB-E7C32384627A} 3.0
typelib FreeLibSrc "%com%\UI\FreeLib (misc)\FreeLib.ocx"
FreeLibSrc.FreeLib f._create
ChDir "%com%\UI\FreeLib (misc)"
 #opt dispatch 1

 BSTR b="Takas"
 f.ConnectTo(&b)

 f._setevents("FreeLib")
 f.Disk="C"
 f.FindFiles("*.*")

 BSTR s="Takas"
 f.ConnectTo(s)

out f.CPUCount

 opt waitmsg 1
 30
