for _i 0 ah.len
	__DIALOGHANDLE& dh=ah[_i]
	sel dh.flags
		case 1 DestroyIcon(dh.handle)
		case 2 DeleteObject(dh.handle)

if(tt) tt._delete
if(colors) colors._delete
if(haccel) DestroyAcceleratorTable(haccel)
if(haccel2) DestroyAcceleratorTable(haccel2)

 notes:
 Don't need to destroy menu.
 Dtor is called 2 times for a dialog. It's normal. We use 2 variables; the first is empty now.
