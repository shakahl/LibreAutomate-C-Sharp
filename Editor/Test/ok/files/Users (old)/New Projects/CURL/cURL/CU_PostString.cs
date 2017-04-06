 /
function# $url str*sin [put] [urlencode]
#compile "CU_def"

 This probably will not work. I don't know how server handles it.

 Uploads sin to url.
 If put is 0, uses POST method, else uses PUT method.
 Returns 1 on success, 0 on failure.

type CUPFDATA str*sin offset
CUPFDATA cd; cd.sin=sin

int r
if(!sin) ret
if(urlencode)
	lpstr es=curl_escape(*sin sin.len)
	*sin=es; curl_free(es)

CURL handle=curl_easy_init; if(!handle) ret

if(curl_easy_setopt(handle CURLOPT_URL url)) goto cl
if(curl_easy_setopt(handle CURLOPT_UPLOAD 1)) goto cl
if(put)
	if(curl_easy_setopt(handle CURLOPT_PUT 1)) goto cl
	if(curl_easy_setopt(handle CURLOPT_INFILESIZE sin.len)) goto cl
else
	if(curl_easy_setopt(handle CURLOPT_POST 1)) goto cl
	if(curl_easy_setopt(handle CURLOPT_POSTFIELDSIZE sin.len)) goto cl
if(curl_easy_setopt(handle CURLOPT_READDATA &cd)) goto cl
if(curl_easy_setopt(handle CURLOPT_READFUNCTION &CU_PostProc)) goto cl
r=!curl_easy_perform(handle)
 cl
curl_easy_cleanup(handle)
ret r
