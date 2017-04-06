function!* hwnd

opt noerrorshere 1

if(!m_memSize) Init(hwnd)
ret m_pm.address
