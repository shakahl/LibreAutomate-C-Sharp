 /exe

 PF
 _s.getfile("$windows$\media\delta\Windows Balloon.wav")
 _s.getfile(":22 $windows$\media\delta\Windows Balloon.wav")
 _s.getfile("resource:<LoadFileDataFromResources (str.getfile, bee)>Windows Balloon.wav")
 _s.getfile("resource:Windows Balloon.wav")
 PN;PO
 out _s.len

 bee 1
 bee+ "$windows$\media\delta\Windows Balloon.wav"
 bee+ ":22 $windows$\media\delta\Windows Balloon.wav"
 bee+ "resource:<LoadFileDataFromResources (str.getfile, bee)>Windows Balloon.wav"
 bee+ "resource:Windows Balloon.wav"
 _s=":22 $windows$\media\delta\Windows Balloon.wav"; bee+ ":22"

 ICsv x._create
 x.Separator=";"
  x.FromFile("$my qm$\test.csv")
 x.FromFile("resource:test.csv")
 x.ToString(_s)
 out _s


 BEGIN PROJECT
 main_function  LoadFileDataFromResources (str.getfile, bee)
 exe_file  $my qm$\LoadFileDataFromResources (str.getfile, bee).qmm
 flags  23
 guid  {B805B36C-2B8C-4776-8AB6-CD0652793D22}
 END PROJECT

