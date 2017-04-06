dll "qm.exe" #ResizeSafeArray ARRAY*a n vt

ARRAY(int) a
if(ResizeSafeArray(&a 3 VT_INT)) end "error"
for(_i 0 a.len) a[_i]=_i
if(ResizeSafeArray(&a 4 VT_INT)) end "error"
for(_i 0 a.len) out a[_i]

ARRAY(BSTR) b
if(ResizeSafeArray(&b 3 VT_BSTR)) end "error"
for(_i 0 b.len) b[_i]=F"string {_i}"
if(ResizeSafeArray(&b 4 VT_BSTR)) end "error"
for(_i 0 b.len) out b[_i]
