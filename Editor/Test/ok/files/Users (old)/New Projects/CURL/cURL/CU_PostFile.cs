 /
function# $url $file [put]
#compile "CU_def"

 This probably will not work. I don't know how server handles it.

 Uploads file to url.
 If put is 0, uses POST method, else uses PUT method.
 Returns 1 on success, 0 on failure.

str s.searchpath(file); if(!s.len) ret
int fp=fopen(s "rb"); if(!fp) ret
int r
CURL handle=curl_easy_init; if(!handle) ret

if(curl_easy_setopt(handle CURLOPT_URL url)) goto cl
if(curl_easy_setopt(handle CURLOPT_UPLOAD 1)) goto cl
if(curl_easy_setopt(handle iif(put CURLOPT_PUT CURLOPT_POST) 1)) goto cl
if(curl_easy_setopt(handle CURLOPT_READDATA fp)) goto cl
r=!curl_easy_perform(handle)
 cl
curl_easy_cleanup(handle)
fclose(fp)
ret r
