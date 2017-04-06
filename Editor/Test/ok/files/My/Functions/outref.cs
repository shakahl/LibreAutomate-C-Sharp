 /
function i

 Displays COM object's reference count.


IUnknown u
memcpy &u &i 4
u.AddRef; out u.Release
memset &u 0 4
