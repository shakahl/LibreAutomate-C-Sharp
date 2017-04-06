function# CDropTarget*pthis GUID*iid !*pObject
*pObject=0
if(memcmp(iid uuidof(IUnknown) sizeof(GUID)) and memcmp(iid uuidof(IDropTarget) sizeof(GUID))) ret E_NOINTERFACE
*pObject=pthis
