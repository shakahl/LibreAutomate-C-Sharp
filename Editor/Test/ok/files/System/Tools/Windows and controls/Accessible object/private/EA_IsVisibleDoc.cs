 /
function! Acc'a [int&browser] [Acc&aDoc]

 Returns 1 if a is in visible web page in one of 3 browsers.

 browser - receives nonzero if container's class is like in one of browsers: 1 IES, 2 FF, 3 Chrome. Even if func returns 0.
 aDoc - receives DOCUMENT.


if(&browser) browser=0

int h=child(a.a); err ret
int i=WinTest(h "Internet Explorer_Server[]Mozilla*[]Chrome*")
if(!i) ret
if(&browser) browser=i
if i=1
	if(&aDoc) aDoc=acc("" "PANE" h "" "" 0x1000);; err ret
	ret 1

 if FF, at first get root doc (main or frame), because acc has problems with combobox items
FFNode f.FromAcc(a)
f.Root(a)
err+

Acc ad
rep
	if(a.Role=ROLE_SYSTEM_DOCUMENT) ad=a
	a.Navigate("parent"); err break

if ad.a and ad.State&STATE_SYSTEM_INVISIBLE=0
	if(&aDoc) aDoc=ad
	ret 1

err+
