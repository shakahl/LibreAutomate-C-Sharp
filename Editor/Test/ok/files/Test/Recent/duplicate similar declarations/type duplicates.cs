 type T1 a b
 type T1 a b
 type T1  a  b
 type T1
	 a
	 b
 type T1 a c


 WINAPI.IBinding b
 
 interface# IBinding :IUnknown
	 Abort()
	 Suspend()
	 Resume()
	 SetPriority(nPriority)
	 GetPriority(*pnPriority)
	 GetBindResult(GUID*pclsidProtocol *pdwResult @**pszResult *pdwReserved)
	 {79eac9c0-baf9-11ce-8c82-00aa004ba90b}
 
 interface# IBinding :IUnknown Abort() Suspend() Resume() SetPriority(nPriority) GetPriority(*pnPriority) GetBindResult(GUID*pclsidProtocol *pdwResult @**pszResult *pdwReserved) {79eac9c0-baf9-11ce-8c82-00aa004ba90b}
