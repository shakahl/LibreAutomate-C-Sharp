out

IStringMap mrem=CreateStringMap
mrem.AddList("DefHookProc[]GetWindowTask[]NdrMarshCCtxtHdl[]NdrUnMarshCCtxtHdl[]BufferedPaintMakeOpaque[]DPA_AppendPtr[]DPA_SortedInsertPtr[]DSA_AppendItem[]FD_ISSET[]FreeModule[]__UA_STACKCOPY[]LockSegment[]UnlockSegment[]LHashValOfName[]SHAnsiToTChar[]SHTCharToAnsi[]SHTCharToUnicode[]SHUnicodeToTChar[]EnumTaskWindows")

str c="#include <windows.h>[]#include <commctrl.h>[]#include <shlwapi.h>[]#include <shlobj.h>[]"
CH_C_macro_to_dll "$qm$\winapi.txt" c mrem 1
CH_C_macro_to_dll "$qm$\winapiv.txt" c mrem 1
 CH_C_macro_to_dll "$qm$\winapi7.txt" c mrem 1 ;;not found
 ret

c.replacerx("\(const \w+\*\)")
lpstr repl=
 (prc)->`((RECT*)prc)->`0
 SendMessage((HWND)hwndCtl,(0x1500+6),0,0));`SendMessage((HWND)hwndCtl,(0x1500+6),0,0);
 SendMessage((HWND)hwndCtl,(0x1500+5),ichStart,ichEnd));`SendMessage((HWND)hwndCtl,(0x1500+5),ichStart,ichEnd);
 int CopyCursor(pcur) { return CopyIcon(pcur)); }`HCURSOR CopyCursor(HICON pcur) { return CopyIcon(pcur); }
 int GlobalDiscard(h)`HGLOBAL GlobalDiscard(HGLOBAL h)
 int LocalDiscard(h)`HGLOBAL LocalDiscard(HGLOBAL h)

str s s1 s2 s3
foreach s repl
	int n=tok(s &s1 3 "`")
	if(n<2) s2=""
	c.findreplace(s1 s2 iif(n>=3 val(s3) 4))

 out c
 ret

 c.setmacro("CH_C_macro_source_all")

UnloadDll "C_macro"
 __Tcc x.Compile("*CH_C_macro_source" "$qm$\C_macro.dll" 2 "comctl32[]shlwapi" "" 0 "Q:\Tools, source, ocx\Misc\w32api-3.14-mingw32\include" "" "Q:\Tools, source, ocx\Misc\w32api-3.14-mingw32\lib")
c.setclip

 This macro adds most C_macro, not only for common controls.
 But it is not very useful. Adds not many C macros, and they can be simply replaced with other functions. Also adds dependenccies on several dlls. Currently not used.
 Tcc does not compile it. Does not find some functions. Compile wih VC.
 Also need to edit/delete something in VC.
