function# *ppSystemFunction

int i

 sel m_method
	 case 1
	 ret ApiUnhook_Mhook(ppSystemFunction)
	 
	 case 2
	 for(i 0 m_map.len) if(m_map[i].x=*ppSystemFunction) *ppSystemFunction=m_map[i].y; break
	 if(i=m_map.len) ret
	  out *ppSystemFunction
	 UnhookFunction(*ppSystemFunction)
	 ret 1
	 
	 case 3
	 for(i 0 m_map.len) if(m_map[i].x=*ppSystemFunction) *ppSystemFunction=m_map[i].y; break
	 if(i=m_map.len) ret
	  out *ppSystemFunction
	 ret !MH_DisableHook(*ppSystemFunction)
	 
	 case 4
	 int pHookFunc
	 for(i 0 m_map.len) if(m_map[i].x=*ppSystemFunction) pHookFunc=m_map[i].y; break
	 if(i=m_map.len) ret
	  out *ppSystemFunction
	 ret ApiUnhook_NCH(pHookFunc)
	  cannot restore *ppSystemFunction
	 
	 case 5
	 ret 1
