class __UseComUnregistered -m_hctx -m_cookie

dll- kernel32
	#CreateActCtxW ACTCTXW*pActCtx
	#ActivateActCtx hActCtx *lpCookie
	#DeactivateActCtx dwFlags ulCookie
	ReleaseActCtx hActCtx
