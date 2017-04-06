/exe

dll "q:\app\qmplus.dll" #TestQmplus [x]

 _i/0
 TestQmplus
 err out _error.description
sub.Sub

rep 2
	out 1
	 _i/0
	 TestQmplus
	 err out _error.description
	out 2


#sub Sub
TestQmplus

 str s="timing"
 s.stem()
 out s

 BEGIN PROJECT
 main_function  Macro2637
 exe_file  $my qm$\Macro2637.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {D45FF931-0A66-4777-8544-52F68812CB3F}
 END PROJECT

 assertion when making exe (not qmm):
 <macro "VS_goto /q:\app\identifiers.cpp(172)">identifiers.cpp(172) : RIdentifiers::Add(pcm=[0xDE32C], idname=[0x48A10B8], idtype=[0x10], data=[0x19], defid=[0x3286], defplace=[0x0], appendToName=[0x0]);  Locals: ph=[0xCCCCCCCC], fc=[0xCCCCCCCC], nn=[0xCCCCCCCC], ie=[0xCCCCCCCC], i=[0xCCCCCCCC];  Thread: <main>
 While auto-adding global var for lock.
 Not always. Only when making exe first time in qm process.
