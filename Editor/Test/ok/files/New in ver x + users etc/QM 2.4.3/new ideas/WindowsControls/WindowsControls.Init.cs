function hwnd [memSize]

opt noerrorshere 1

_i=iif(memSize>4096 memSize 4096)
m_pm.Alloc(hwnd _i)
m_memSize=memSize
m_is64Bit=IsWindow64Bit(hwnd)
