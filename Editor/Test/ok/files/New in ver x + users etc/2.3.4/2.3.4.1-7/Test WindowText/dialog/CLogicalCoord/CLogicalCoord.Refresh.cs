
 Call this after changing logical coordinates (org, ext, etc).

Offset(0 0)

ex=1; ey=1
sel GetMapMode(m_hdc)
	case [MM_ANISOTROPIC,MM_ISOTROPIC] GetViewportExtEx m_hdc +&ex
	case else
	RECT r; SetRect &r 0 0 1 1; DPtoLP m_hdc +&r 2
	ex=r.right-r.left ;;ey always 1
