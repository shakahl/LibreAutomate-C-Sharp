 Note:
 Don't use this.
 Would be useful only for common control C macros.
 Now works, but there are some problems, eg there are no W macros, only A.
 Does not simplify. Using messages also is OK.
 Also, requires extra dll, which is not good for exe.
 Also would need to test all, maybe there are bugs. Much work.

out

str c="#include <windows.h>[]"
CH_C_macro_to_dll "$qm$\winapi.txt" c
CH_C_macro_to_dll "$qm$\winapiv.txt" c
 CH_C_macro_to_dll "$qm$\winapi7.txt" c ;;not found
 ret

c.replacerx("\(const \w+\*\)")
lpstr repl=
 (prc)->`((RECT*)prc)->`0
 SendMessage((HWND)hwndCtl,(0x1500+6),0,0));`SendMessage((HWND)hwndCtl,(0x1500+6),0,0);
 SendMessage((HWND)hwndCtl,(0x1500+5),ichStart,ichEnd));`SendMessage((HWND)hwndCtl,(0x1500+5),ichStart,ichEnd);
str s s1 s2 s3
foreach s repl
	int n=tok(s &s1 3 "`")
	if(n<2) s2=""
	c.findreplace(s1 s2 iif(n>=3 val(s3) 4))

 out c
 ret

c.setmacro("CH_C_macro_source")

UnloadDll "C_macro"
 __Tcc x.Compile("*CH_C_macro_source" "$qm$\C_macro.dll" 2)
c.setclip

 Better compile the code with VC, because it makes smaller dll. Tcc 30 kB, VC 23 kB.
