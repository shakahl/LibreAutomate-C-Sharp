 /
function# $url str*sout
#compile "CU_def"

 Downloads url into sout.
 Returns 1 on success, 0 on failure.

int r
if(!sout) ret
CURL handle=curl_easy_init; if(!handle) ret

if(curl_easy_setopt(handle CURLOPT_URL url)) goto cl
if(curl_easy_setopt(handle CURLOPT_WRITEDATA sout)) goto cl
if(curl_easy_setopt(handle CURLOPT_WRITEFUNCTION &CU_GetProc)) goto cl
r=!curl_easy_perform(handle)
 cl
curl_easy_cleanup(handle)
ret r
