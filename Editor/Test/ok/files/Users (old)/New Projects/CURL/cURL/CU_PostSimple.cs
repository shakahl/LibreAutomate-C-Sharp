 /
function# $url $text
#compile "CU_def"

 Simple http post.
 text is url-encoded text, like "text=abc&button=OK"
 Returns 1 on success, 0 on failure.

int r closehandle
if(!handle) closehandle=1; handle=curl_easy_init; if(!handle) ret

if(url and curl_easy_setopt(handle CURLOPT_URL url)) goto cl
if(curl_easy_setopt(handle CURLOPT_POSTFIELDS text)) goto cl
r=!curl_easy_perform(handle)
 cl
if(closehandle) curl_easy_cleanup(handle)
ret r
