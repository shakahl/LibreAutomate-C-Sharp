
 Gets object from mouse, and initializes this variable.
 If it is TEXT, STATICTEXT or GRAPHIC and its parent object is LINK, gets the parent object.
 Returns 1 if the object finally is LINK, 0 if not.


this=acc(mouse)
err end ERR_OBJECTGET

sel Role
	case ROLE_SYSTEM_LINK
	ret 1
	
	case [ROLE_SYSTEM_TEXT,ROLE_SYSTEM_STATICTEXT,ROLE_SYSTEM_GRAPHIC]
	Acc b; Navigate("parent" b)
	if b.Role=ROLE_SYSTEM_LINK
		this=b
		ret 1
	else ;;support 2 levels, eg youtube right-side list
		b.Navigate("parent")
		if b.Role=ROLE_SYSTEM_LINK
			this=b
			ret 1
err+
