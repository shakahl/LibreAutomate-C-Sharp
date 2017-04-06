function [flags] ;;flags: 1 prefer LINK

 Gets object from mouse, and initializes this variable.

 flags (QM 2.4.3.8):
   1 - if the object is TEXT, STATICTEXT or GRAPHIC and its parent object is LINK, get the parent object.

 Added in: QM 2.3.3.


this=acc(mouse)
err end ERR_OBJECTGET

if flags&1
	sel Role
		case [ROLE_SYSTEM_TEXT,ROLE_SYSTEM_STATICTEXT,ROLE_SYSTEM_GRAPHIC]
		Acc b; Navigate("parent" b)
		if b.Role=ROLE_SYSTEM_LINK
			this=b
		else ;;support 2 levels, eg youtube right-side list
			b.Navigate("parent")
			if b.Role=ROLE_SYSTEM_LINK
				this=b
err+
