 /
function# [$url] [flags] ;;flags: 1 faster

 Waits while Firefox is busy (loading page).
 Returns window handle.
 Error if fails, eg if Firefox closed while waiting.
 Tested with Firefox 2.0, 3.0, 3.6, 4.0 and 15.0. Should also work with other versions.

 url - web page address to open.
   If empty, just waits while Firefox is busy.
   Can include command line, eg "-safe-mode ''url''".
 flags:
   1 stop waiting faster. Less reliable. Without this flag with some web pages would wait too long.

 May not wait if this function opens new window when a window already exists.


int w1=WB_Open("firefox" url "Firefo" "Mozilla*WindowClass" 0x804)

 wait until DOCUMENT is available and its state is not busy
int i n busy
rep
	i+1
	0.1
	if(!IsWindow(w1)) end ES_FAILED
	busy=1
	IAccessible a=0
	if(!AccessibleObjectFromWindow(w1 OBJID_CLIENT IID_IAccessible &a)) a=a.Navigate(0x1009 1); err a=0
	if(a) busy=a.State(0)&STATE_SYSTEM_BUSY; err
	
	 out F"0x{busy} {n}"
	
	if(busy) n=0
	else n+1; if(n=12 or (flags&1 and i>=20 and n>1)) break

ret w1
err+ end ES_FAILED

 notes:
 This way is documented, works in all FF versions (not tested in 1.5), but not 100% reliable.
 With some pages stops waiting faster than FF changes its UI state to "Done". Not always.
   But visually then page is loaded, and acc finds objects.
 With some pages, after first "not busy", many times Navigate either succeeds or fails.
   When fails, acc also does not find DOCUMENT and other objects in it.
   Example: download.com. Bad in FF3.6, good in FF4.
   Then can use flag 1 to not wait more.
 But with most tested pages works well.
 Another way would be to wait for Stop or Refresh button text or state.
   Too unreliable, because: different UI in FF versions; can change language; can delete or move to other toolbar.
