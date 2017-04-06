function# hwnd

 Returns nonzero if the window is cloaked.
 Returns flags: 0 not cloaked, 1 cloaked by its application, 2 cloaked by Windows, 4 parent window is cloaked.

 REMARKS
 On Windows 8 and later, a window may be cloaked. Then it is invisible, although functions IsWindowVisible and hid say it is visible.
 On Windows 8, cloaked are inactive Windows store app windows. Also some inactive system windows, such as Start screen and Search.
 On Windows 10, cloaked are windows on inactive desktops. Also some system windows.

 Added in: QM 2.4.3.


if(_winver>=0x602 and !DwmGetWindowAttribute(hwnd 14 &_i 4)) ret _i
