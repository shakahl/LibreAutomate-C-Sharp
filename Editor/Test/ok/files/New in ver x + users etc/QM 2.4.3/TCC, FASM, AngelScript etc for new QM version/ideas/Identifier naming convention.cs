	IK_KEYWORD:					keyword
	IK_FUNCTION:
		Global, dll:			Func, MyFunc
		Class member:
			Public:				Func, MyFunc
			Protected:			_Func, _MyFunc
			Static public:		Func_, MyFunc_, but can be accessed with DoThis
			Static protected:	_Func_, _MyFunc_
	IK_VARIABLE,
		Local, struct member:	i, aParam, A, _s (any that does not begin with an ucase char, or is single ucase char)
		Global:					g_var
		Static:					s_var
		Thread:					t_var
		Class member:
			Public:				aMember
			Protected:			_aMember
			Static public:		aMember_
			Static protected: 	_aMember_
	IK_CONSTANT, enum member:	MY_CONSTANT
	IK_ENUM:					eEnum, eENUM
	IK_TYPE:
		Struct:					STRUCT, MY_STRUCT
		Class, CoClass:			Class, MyClass
		Interface:				IKnown
		Function type:			fFunc
	IK_TYPELIB:					Typelib, MyTypelib
	IK_REF:						Ref, MyRef
	IK_DIRECTIVE:				
	IK_ALIAS:					like the alias

Don't need to specify public/protected for class member functions and variables. If name begins with _, it is protected, else public.
