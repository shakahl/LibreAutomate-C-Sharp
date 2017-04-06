 Before running this, go to the QM forum RSS page, say Yes for 'Remove your posts...', generate URL, and use it in this function for rssurl variable.
 Also need to do this after changing your RSS password.


60 ;;check first time after 1 munute
int period=5 ;;checks every period minutes
str rssurl="http://www.quickmacros.com/forum/smartfeed.php?u=2&e=ivNnynxk/1yvaJJZJ5QwJTL6D0cj08TWKYifX00WvCbf3rQgqGNBgg==&removemine=1&feed_type=RSS2.0&limit=LF&sort_by=user&feed_style=HTML&lastvisit=1&filter_foes=1&max_word_size=All"
 str openurl="http://www.quickmacros.com/forum/" ;;when found new item in rss, opens this url in web browser. If this url is empty, opens url of the last item.
str openurl="http://www.quickmacros.com/forum/search.php?search_id=active_topics" ;;when found new item in rss, opens this url in web browser. If this url is empty, opens url of the last item.

str s lasturl
rget lasturl "rss_lasturl"
IXml xml=CreateXml
rep
	IntGetFile rssurl s; err sub.Error; goto next
	 out s
	xml.FromString(s); err sub.Error; out s; goto next
	IXmlNode n=xml.Path("rss/channel/item/link") ;;get url of the last post
	if(!n) goto next ;;nothing new
	s=n.Value
	out s
	if(s=lasturl) out "<same as lasturl>"; goto next ;;nothing new
	if(s="http://www.quickmacros.com/forum/smartfeed_url.php") ;;rss notifier error
		s=n.Parent.Child("description").Value
		out "RssNotifier error: %s" s
		sub.Error s
		goto next
	int w=win; if(w and wintest(w "Adobe Flash Player" "ShockwaveFlashFullScreen")) key Z; 0.3
	run "firefox.exe" iif(openurl.len openurl s)
	err goto next ;;ocassionally run fails with message box saying that cannot find the path (url). Next time should work well. It happens after returning from hibernation.
	lasturl=s
	rset lasturl "rss_lasturl"
	 next
	60*period


#sub Error
function [$errStr]

if(empty(errStr)) errStr=F"'{_error.description}' at {_error.line}"
out errStr
LogFile F"RssNotifier error: {errStr}" 1
