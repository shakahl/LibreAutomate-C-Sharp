 /
function# ARRAY(str)&a

 Gets full paths of files copied to the clipboard.
 Returns number of files.

 EXAMPLE
 ARRAY(str) a
 GetClipboardFiles a
 int i
 for i 0 a.len
	 out a[i]


int i n

if(!OpenClipboard(0)) ret
int h=GetClipboardData(CF_HDROP)
CloseClipboard
if(!h) ret

n=DragQueryFileW(h -1 0 0) ;;how many
a.create(n)
BSTR b.alloc(300)
for i 0 n
	DragQueryFileW(h i b 300)
	a[i].ansi(b)
ret n
