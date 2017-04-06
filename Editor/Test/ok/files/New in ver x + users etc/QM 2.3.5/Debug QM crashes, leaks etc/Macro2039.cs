 /exe
 out 1
byte* p=+LocalAlloc(0 1000000)
 rep(2) LocalFree(p)

 lstrlenW(L"string")
lstrlenW(+"")
 FormatKeyString VK_F5 1 &_s
 out _s

 int w=wait(3 WV win("Google - Windows Internet Explorer" "IEFrame"))
 Htm e=htm("SPAN" "Man sekasi!" "" w "0" 31 0x21 3)
 3
 rep 2
	 int w=wait(3 WV win("Quick Macros Online Help - Windows Internet Explorer" "IEFrame"))
	 Htm e=htm("A" "QmSetWindowClassFlags" "" w "2" 1 0x21 3)
	 e.el.AddRef
 5

 MSHTML.IHTMLDocument2 doc=e.el.document
 outref doc

 out e.el.Release
 memset &e.el 0 4

 BEGIN PROJECT
 main_function  Macro2039
 exe_file  $qm$\memleak.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {5483AF29-9BB8-4582-BF6D-EBBF2FC45DBA}
 END PROJECT
