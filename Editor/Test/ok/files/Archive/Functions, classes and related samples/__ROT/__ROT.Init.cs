function#

IEnumMoniker e
IMoniker m

if m_rot
	m_a=0
else
	if(GetRunningObjectTable(0 &m_rot)) ret
	CreateBindCtx(0 &m_bind)

m_rot.EnumRunning(&e)

rep
	e.Next(1 &m &_i)
	if(_hresult) break
	m_a[]=m; m=0

err+
ret m_a.len
