function'Acc `hwnd [flags] [^waits] ;;flags: 1 error, 2 web page, 4 reset, 128 reverse

 Finds accessible object, and initializes this variable.
 Returns: accessible object, or 0 if not found. If flag 1, error if not found.

 hwnd - where to search.
   Can be one of:
     Window handle, name or +class. If "" - active window. Can be top-level or child window. Must not be child if flag 0x2000.
     IAccessible variable (accessible object). If you have Acc a, pass a.a.
     FFNode variable.


opt noerrorshere 1
int i n=m_a.len
if(!n) end ERR_BADARG
Acc ar
 gw
for i 0 n
	__ACCPATH& r=m_a[i]
	r.flags~0x1000; r.flags|64|(flags&128); if(flags&2 and !i) r.flags|0x2000
	if(i) hwnd=ar.a
	ar.Find(hwnd r.role r.name r.prop r.flags 0 r.matchIndex r.navig r.cbFunc r.cbParam)
	if(!ar.a) break
	 TODO: search in all possible path, not only in the first found

if i=n
	if(flags&4) m_a=0
	ret ar

if(waits>0) waits-0.1; 0.1; goto gw
if(flags&1) end ERR_OBJECT
