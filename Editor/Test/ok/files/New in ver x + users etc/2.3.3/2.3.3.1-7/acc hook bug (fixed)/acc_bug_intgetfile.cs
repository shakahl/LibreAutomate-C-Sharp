str url="http://www.quickmacros.com/forum/smartfeed.php?u=2&e=ivNnynxk/1yvaJJZJ5QwJTL6D0cj08TWKYifX00WvCbf3rQgqGNBgg==&removemine=1&feed_type=RSS2.0&limit=LF&sort_by=user&feed_style=HTML&lastvisit=1&filter_foes=1&max_word_size=All"
 str url="http://www.quickmacros.com/test.html"
 str url="http://www.quickmacros.com/forum"
str s
 Q &q
 IntGetFile url s
 Sleep 1000
 Q &qq
 outq
 ret

__HInternet m_hitop
m_hitop=InternetOpen("Quick Macros" 0 0 0 0)

int inetflags=INTERNET_FLAG_RELOAD|INTERNET_FLAG_NO_CACHE_WRITE
__HInternet hi=InternetOpenUrl(m_hitop url 0 0 inetflags 0)
if(!hi) ret
out hi
