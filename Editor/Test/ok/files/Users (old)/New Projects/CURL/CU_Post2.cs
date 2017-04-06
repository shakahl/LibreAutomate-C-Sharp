function# $url $text
#compile "CU_def"

 Performs http post (submits form).
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

 I don't know exactly, but if url file has this:

 <form method="POST" action="some.cgi">
   <input type=text name="text">
   <input type=submit name="button" value="OK">
 </form>

 then cgi will receive text.
 To send file, use CU_PutFile. Then url file should contain this:

 <form method="POST" enctype='multipart/form-data' action="some.cgi">
   <input type=file name="upload">
   <input type=submit name="button" value="OK">
 </form>
