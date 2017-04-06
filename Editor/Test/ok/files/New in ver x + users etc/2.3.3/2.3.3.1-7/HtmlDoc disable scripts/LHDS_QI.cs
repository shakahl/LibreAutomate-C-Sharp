function# _ GUID*iid *p
out __FUNCTION__
*p=0
 if(memcmp(iid uuidof(IUnknown) sizeof(GUID)) and memcmp(iid uuidof(IDropTarget) sizeof(GUID))) ret E_NOINTERFACE
*p=_
