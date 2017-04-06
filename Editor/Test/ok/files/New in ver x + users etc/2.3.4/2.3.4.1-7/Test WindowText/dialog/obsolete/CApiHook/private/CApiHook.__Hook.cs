function# *ppSystemFunction pHookFunction

POINT p
int R

sel m_method
	 case 1
	 ret ApiSetHook_Mhook(ppSystemFunction pHookFunction)
	 
	 case 2
	  out *ppSystemFunction
	 p.y=*ppSystemFunction
	 if(!HookFunction(*ppSystemFunction pHookFunction)) ret
	 *ppSystemFunction=GetOriginalFunction(pHookFunction)
	 p.x=*ppSystemFunction; m_map[]=p
	 
	 case 3
	 p.y=*ppSystemFunction
	 if(MH_CreateHook(*ppSystemFunction pHookFunction ppSystemFunction)) ret
	 if(MH_EnableHook(p.y)) ret
	 p.x=*ppSystemFunction; m_map[]=p
	 
	 case 4
	  out *ppSystemFunction
	 p.y=pHookFunction
	 p.x=ApiSetHook_NCH(*ppSystemFunction pHookFunction); if(!p.x) ret
	 *ppSystemFunction=p.x
	 p.x=*ppSystemFunction; m_map[]=p
	
	case 5
	p.y=*ppSystemFunction
	if(AH_CreateHook(*ppSystemFunction pHookFunction ppSystemFunction)) ret
	p.x=*ppSystemFunction; m_map[]=p

ret 1
