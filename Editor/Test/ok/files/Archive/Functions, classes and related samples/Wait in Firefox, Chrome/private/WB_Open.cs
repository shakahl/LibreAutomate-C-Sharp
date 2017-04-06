 /
function# $exe $url $winName $winClass winFlags

 Opens url in exe and returns window handle.


spe 500
int w1
if empty(url)
	w1=win(winName winClass "" winFlags)
	if(!w1) run exe
else
	run exe url
	w1=wait(5 WA win(winName winClass "" winFlags)); err

w1=wait(60 WV win(winName winClass "" winFlags))

ret w1

err+ end ES_FAILED
