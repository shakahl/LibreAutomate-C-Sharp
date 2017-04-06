function __GlobalMem&r

if(handle) GlobalFree handle; handle=0
handle=GlobalAlloc(0 GlobalSize(r.handle))
