 /
function! hwnd

 Returns 1 if the window is on the currently active virtual desktop, 2 if on all desktops. Returns 0 if it is on an inactive desktop.

 REMARKS
 Virtual desktops is a Windows 10 feature. On older OS this function always returns 1.

 
if(_winver<0xA00) ret 2

#compile "IVirtualDesktopManager"

if(!IsWindow(hwnd)) ret
if(!IsWindowCloaked(hwnd)) ret 1 ;;much faster

int ho=GetAncestor(hwnd 3); if(!ho) ret ;;would fail if owned window. Tested: an owned window cannot be on a different desktop than its owner.
if ho!=hwnd
	int hp=GetAncestor(ho 2)
	if(hp!=ho) ho=GetAncestor(hp 3); if(!ho) ret ;;QM toolbar owners are child windows with ws_popup style, and GetAncestor(hwnd 3) does not get their owner. Tested: GA gets root owner if direct owner is normal child window (ws_child).
	hwnd=ho
 tested: GA returns hwnd if hwnd is owned by desktop, except if it has ws_child style, eg ComboLBox.

 IVirtualDesktopManager_Raw-- m._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}")
 IVirtualDesktopManager_Raw- t_vdeskman._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}")
IVirtualDesktopManager_Raw m; if(!__GetVirtualDesktopManager(m)) ret

int R
if(m.IsWindowOnCurrentVirtualDesktop(hwnd R)) ret 2 ;;if fails, the window does not have a desktop assigned, then assume that it is on all desktops
ret R

 TODO: test how when the desktop switcher is active.
