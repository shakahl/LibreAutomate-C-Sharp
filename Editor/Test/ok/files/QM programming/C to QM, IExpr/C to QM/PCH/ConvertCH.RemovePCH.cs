 /CtoQM
str k
m_mpch.EnumBegin
rep
	if(!m_mpch.EnumNext(k 0)) break
	m_mall.Remove(k)
	m_mo.Remove(k)

lpstr s=
 GetStringType
 IntlStrEqWorker
 UrlIsNoHistory
foreach k s
	m_mall.Remove(k)
	m_mo.Remove(k)
