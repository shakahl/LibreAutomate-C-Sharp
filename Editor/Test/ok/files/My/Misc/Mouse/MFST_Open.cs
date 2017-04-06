 /
function# $url [flags] ;;flags: 1 wait

 Opens URL in IE.
 Uses separate IE window.
 Returns IE window handle.


int w1=FindTaggedWindow("qm_mfst" " Internet Explorer" "IEFrame")
if w1
	act w1
else
	run "iexplore.exe" "" "" "" 0x2800 win(" Internet Explorer" "IEFrame") w1
	TagWindow w1 "qm_mfst"
	wait 30 WC child("" "Internet Explorer_Server" w1)

web url flags&1 w1

ret w1
