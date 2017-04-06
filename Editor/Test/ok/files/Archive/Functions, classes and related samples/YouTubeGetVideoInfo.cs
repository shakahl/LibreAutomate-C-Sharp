 /
function $videoID [str&title] [str&description] [str&author]

 Gets YouTube video title, description, author.
 Error if fails.

 videoID - video URL part that in video URL usually is after v=.
 title, description, author - variables that receive video info. Can be 0.

 EXAMPLE
 str title description author
 YouTubeGetVideoInfo "ZooYDKIDOaQ" title description author
 out F"title:[]{title}[]description:[]{description}[]author:[]{author}"


str s
IntGetFile F"http://gdata.youtube.com/feeds/api/videos/{videoID}" s

IXml x=CreateXml
x.FromString(s)
if(&title) title=x.Path("entry/title").Value
if(&description) description=x.Path("entry/content").Value
if(&author) author=x.Path("entry/author/name").Value

err+ end _error
