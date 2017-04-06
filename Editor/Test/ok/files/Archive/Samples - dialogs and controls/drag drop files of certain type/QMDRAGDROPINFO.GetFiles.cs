function# ARRAY(str)&a [flags] ;;flags: 1 free memory (called on drop)

 Gets paths of file(s) being dragged or dropped.
 Returns the number of files.
 Does not support non-file objects, such as control panel items.


a=0
int i
for(i 0 formats.len) if(formats[i].cfFormat=CF_HDROP) goto g1
ret
 g1
#opt nowarnings 1
STGMEDIUM sm ;;warning because of composites in union
dataObj.GetData(&formats[i] &sm); err ret
if(sm.tymed!=TYMED_HGLOBAL) ret
BSTR b.alloc(300)
for i 0 DragQueryFileW(sm.hGlobal -1 0 0)
	DragQueryFileW(sm.hGlobal i b 300)
	a[].ansi(b)

if(flags&1) DragFinish sm.hGlobal
ret a.len
