dll "C:\Documents and Settings\G\Desktop\muParser\ParserDLL\muParser.dll"
	#mupInit
	mupRelease a_hParser
	$mupGetExpr a_hParser
	mupSetExpr a_hParser $a_szExpr
	mupSetErrorHandler a_pErrHandler
	mupSetVarFactory a_hParser a_pFactory
	^mupEval a_hParser
	mupDefineFun1 a_hParser $a_szName a_pFun a_bAllowOpt
	mupDefineFun2 a_hParser $a_szName a_pFun a_bAllowOpt
	mupDefineFun3 a_hParser $a_szName a_pFun a_bAllowOpt
	mupDefineFun4 a_hParser $a_szName a_pFun a_bAllowOpt
	mupDefineFun5 a_hParser $a_szName a_pFun a_bAllowOpt
	mupDefineStrFun a_hParser $a_szName a_pFun a_bAllowOpt
	mupDefineMultFun a_hParser $a_szName a_pFun a_bAllowOpt
	mupDefineOprt a_hParser $a_szName a_pFun a_iPri a_bAllowOpt
	mupDefineConst a_hParser $a_szName ^a_fVal
	mupDefineStrConst a_hParser $a_szName $a_sVal
	mupDefineVar a_hParser $a_szName ^*a_fVar
	mupDefinePostfixOprt a_hParser $a_szName a_pOprt a_bAllowOpt
	mupDefineInfixOprt a_hParser $a_szName a_pOprt a_bAllowOpt
	mupDefineNameChars a_hParser $a_szCharset
	mupDefineOprtChars a_hParser $a_szCharset
	mupDefineInfixOprtChars a_hParser $a_szCharset
	mupRemoveVar a_hParser $a_szName
	mupClearVar a_hParser
	mupClearConst a_hParser
	mupClearOprt a_hParser
	#mupGetExprVarNum a_hParser
	#mupGetVarNum a_hParser
	#mupGetConstNum a_hParser
	mupGetExprVar a_hParser $a_iVar $*a_pszName ^**a_pVar
	mupGetVar a_hParser a_iVar $*a_pszName ^**a_pVar
	mupGetConst a_hParser a_iVar $*a_pszName ^*a_pVar
	mupAddValIdent a_hParser fa
	#mupError
	mupErrorReset
	$mupGetErrorMsg
	#mupGetErrorCode
	#mupGetErrorPos
	$mupGetErrorToken
	mupEnableBuiltInOprt a_hParser on
	
def TEST1 1
def TEST2
def TEST3 3
