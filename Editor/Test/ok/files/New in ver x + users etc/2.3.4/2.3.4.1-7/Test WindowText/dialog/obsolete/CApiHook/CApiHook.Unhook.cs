#ret
 /dlg_apihook
if(!m_hooked) ret
m_hooked=0

 if(!__Unhook(+&fnMessageBoxA)) out "failed to unhook"
 if(!__Unhook(+&fnMessageBoxW)) out "failed to unhook"

if(!__Unhook(+&fnExtTextOutW)) out "failed to unhook"
if(!__Unhook(+&fnDrawTextExW)) out "failed to unhook"
if(!__Unhook(+&fnTextOutW)) out "failed to unhook"
if(!__Unhook(+&fnPolyTextOutW)) out "failed to unhook"
if(fnGdipDrawString)
	if(!__Unhook(+&fnGdipDrawString)) out "failed to unhook"
	if(!__Unhook(+&fnGdipDrawDriverString)) out "failed to unhook"
if(!__Unhook(+&fnBitBlt)) out "failed to unhook"
if(!__Unhook(+&fnExtTextOutA)) out "failed to unhook"
if(!__Unhook(+&fnTextOutA)) out "failed to unhook"
if(!__Unhook(+&fnPolyTextOutA)) out "failed to unhook"
if(!__Unhook(+&fnScriptShape)) out "failed to unhook"
#if _winnt>=6
if(!__Unhook(+&fnScriptShapeOpenType)) out "failed to unhook"
#endif
if(!__Unhook(+&fnScriptTextOut)) out "failed to unhook"

 if(!__Unhook(+&fnDrawThemeText)) out "failed to unhook"
 if(!__Unhook(+&fnDrawThemeTextEx)) out "failed to unhook"

 if(m_method=3 and MH_Uninitialize) out "failed to uninit"

if(m_method=5)
	if(AH_DisableHooks) out "failed to disable hooks"
	 0.001
	if(AH_DeleteHooks) out "failed to delete hooks"
