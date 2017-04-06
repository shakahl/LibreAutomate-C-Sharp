function#
int R=InterlockedDecrement(&m_cRef)
if(R) ret R
RefCounted* p=&this
p._delete
