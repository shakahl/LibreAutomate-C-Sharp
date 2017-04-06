 class CApiHook -!m_hooked -!m_method --ARRAY(POINT)m_map

 dll- qmth
	 #ApiSetHook_Mhook *ppSystemFunction pHookFunction
	 #ApiUnhook_Mhook *ppSystemFunction
	 QMTH_CbtHookProc
	 #ApiSetHook_NCH pSystemFunction pHookFunction
	 #ApiUnhook_NCH pHookFunction
 
 dll- nthookengine
	 #HookFunction OriginalFunction HookFunction
	 UnhookFunction OriginalFunction
	 #GetOriginalFunction HookFunction
  
 dll- minhook32.dll
	 #MH_Initialize
	 #MH_Uninitialize
	 #MH_CreateHook pTarget pDetour *ppOriginal
	 #MH_EnableHook pTarget
	 #MH_DisableHook pTarget

 dll- "$qm$\qmtc32.dll"
	 #AH_CreateHook pTarget pDetour *ppOriginal
	 #AH_EnableHooks
	 #AH_DisableHooks
	 #AH_DeleteHooks

#compile "__CLogicalCoord"
 #compile "CApiHook.Hook"

dll- comctl32 #DrawShadowText hdc @*pszText cch RECT*prc dwFlags crText crShadow ixOffset iyOffset
dll- uxtheme #DrawThemeText hTheme hdc iPartId iStateId @*pszText cchText dwTextFlags dwTextFlags2 RECT*pRect
dll- uxtheme #DrawThemeTextEx hTheme hdc iPartId iStateId @*pszText cchText dwTextFlags RECT*pRect DTTOPTS*pOptions
dll- uxtheme #OpenThemeData hwnd @*pszClassList
dll- uxtheme #CloseThemeData hTheme
