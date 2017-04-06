def XCLASS_DATA 0x2000
def XCLASS_FLAGS 0x4000
def XTYP_REQUEST 0x000020B0
def XTYP_POKE 0x00004090
def XTYP_EXECUTE 0x00004050
def CBF_FAIL_EXECUTES 0x00008000
def APPCMD_CLIENTONLY 0x00000010
def CBF_SKIP_ALLNOTIFICATIONS 0x003c0000
def CP_WINUNICODE 1200
def DMLERR_NOTPROCESSED 0x4009

type SECURITY_QUALITY_OF_SERVICE Length @Impersonationlevel @ContextTrackingMode EffectiveOnly
type CONVCONTEXT cb wFlags wCountryID iCodePage dwLangID dwSecurity SECURITY_QUALITY_OF_SERVICE'qos

dll user32
	#DdeInitializeW *pidInst pfnCallback afCmd ulRes
	#DdeCreateStringHandleW idInst @*psz iCodePage
	#DdeConnect idInst hszService hszTopic CONVCONTEXT*pCC
	#DdeClientTransaction !*pData cbData hConv hszItem wFmt wType dwTimeout *pdwResult
	#DdeDisconnect hConv
	#DdeFreeStringHandle idInst hsz
	#DdeUninitialize idInst
	!*DdeAccessData hData *pcbDataSize
	#DdeUnaccessData hData
	#DdeFreeDataHandle hData
	#DdeGetLastError idInst

type ___DdeStr hs -m_idinst

DdeInitializeW(&m_idinst 0 APPCMD_CLIENTONLY|CBF_SKIP_ALLNOTIFICATIONS 0)
