 /
function ARRAY(int)&a

str ss=
 init
 WinConstants
 VirtualKeys
 WinStyles
 WinMessages
 WinTypes
 WinInterfaces
 WinFunctions
 MSVCRT
 References
 QmDef
 QmDll
 Classes
 __ffdom_api
 Classes2
 Categories
 TO_InitToolsControl
 __RegisterWindowClass.
 __RegisterWindowClass.Unregister
 __RegisterWindowClass.Register
 init2
 QM toolbar


a=0
str s
foreach s ss
	a[]=qmitem(s 1)
