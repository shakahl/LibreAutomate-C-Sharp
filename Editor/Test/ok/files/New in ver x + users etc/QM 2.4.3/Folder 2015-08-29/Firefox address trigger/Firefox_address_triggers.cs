 \
int w=TriggerWindow
Acc a
str s sPrev
rep
	0.5
	if(!IsWindow(w)) ret
	if !a.a
		a.Find(w "DOCUMENT" "" "" 0x3000 1); err continue
	s=a.Value; err
	if(_hresult) a.a=0; continue
	if(s=sPrev) continue
	sub.UrlChanged w s sPrev
	sPrev=s


#sub UrlChanged
function w str&url str&previousUrl

 Edit this sub-function. Add/remove case for URLs you need.
 You can edit it while this function is running. Click Compile to apply changes.

out url
sel url 3
	case "https://m.facebook.com/*"
	mac "facebook macro" "" w url previousUrl
	  the macro should begin: function [hwnd] [$url] [$previousUrl]
	
	 case ["url2","url3"]
	 mac "macro2"
	
	 ...

 err+ out _error.description
