 /
function# [hkl] [flags] ;;flags: 1 for this process (else for this thread only)

 Changes the input keyboard layout for this thread or this process.
 Returns previous keyboard layout id. Returns 0 if failed or when hkl is 0.

 hkl - keyboard layout id. Must be one of loaded keyboard layouts.
   If hkl is 0, this function shows ids of all loaded keyboard layouts.
   When there are multiple loaded keyboard layouts, you probably can see their names when you click the current active layout name in the system notification area (aka tray) by the clock.

 REMARKS
 This function is not restricted to keyboard layouts. The hkl parameter is actually an input locale identifier. This is a broader concept than a keyboard layout, since it can also encompass a speech-to-text converter, an Input Method Editor (IME), or any other form of input. Several input locale identifiers can be loaded at any one time, but only one is active at a time. Loading multiple input locale identifiers makes it possible to rapidly switch between them.

 This function cannot change the input keyboard layout for another process. For it you can use hotkeys that are defined in system language settings, for example send Shift+Alt (key SA).

 EXAMPLE
 ActivateInputKeyboardLayout 0xF0270427
 str s
 if(!inp(s)) ret
 out s


if hkl
	int f=0; if(flags&1) f|KLF_SETFORPROCESS
	ret ActivateKeyboardLayout(hkl f)

ARRAY(int) a.create(1000)
int n=GetKeyboardLayoutList(a.len &a[0])
int i
for i 0 n
	out "0x%X" a[i]
