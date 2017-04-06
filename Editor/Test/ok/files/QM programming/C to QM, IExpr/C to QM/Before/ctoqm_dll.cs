interface# IExprC :IUnknown
	%EvalC($expr)
	SetIdMap(IStringMap'm)
	SetIdFunc(fa param)
	{DA2D9350-50FF-444b-8EE9-BF019A498D44}
dll ctoqm
	IExprC'CreateExpr
	$SkipEnclosed $s rc
	#EscapeCString str*s flags ;;flags: 1 text with strings, 2 replace to QM esc
	$FindNextCIdentifier $s *length
	NewLinesToSpaces $s lens
	 !ReadCEscape lpstr&s byte&ch
	 !ReadCCharConst lpstr&s int&R
	#SplitMultiStatementLines str&s
