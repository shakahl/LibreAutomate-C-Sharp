 Keeps msbuild.eae and vbcscompiler.exe processes running, to avoid slow compiling.
 They would exit after some time of not using: vbcscompiler 10 min, msbuild 15 min.
 Opens an empty C# project in a hidden VS process and every 8 min modifies code and builds.

 Sometimes does not work. Better don't use, because also spins CPU every 8 min. Too dirty and unreliable.


int testOnce;;=1

int w=win("KeepMSBuild - Microsoft Visual Studio" "HwndWrapper[DefaultDomain;*")
if(!w)
	run "$program files$\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe"
	w=wait(60 WA win("KeepMSBuild - Microsoft Visual Studio" "HwndWrapper[DefaultDomain;*"))
if !testOnce
	hid w
	5
rep
	Acc a.Find(w "TEXT" "Text Editor" "" 0x1015)
	a.Select(1)
	PostMessage w WM_CHAR 'A' 0
	PostMessage w WM_KEYDOWN VK_F7 0
	if(testOnce) break
	8*60
