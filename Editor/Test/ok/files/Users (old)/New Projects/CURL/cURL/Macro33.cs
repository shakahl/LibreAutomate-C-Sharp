 This is converted from "postit2.c" example.

 Example code that uploads a file to a remote script that accepts
 "HTML form based" (as described in RFC1738) uploads using HTTP POST.
 The imaginary form we'll fill in looks like:

 <form method="post" enctype="multipart/form-data" action="examplepost.cgi">
 <input type="file" name="sendfile" size="40">
 <input type="text" name="filename" size="30">
 <input type="submit" name="submit" value="send">
 </form>

lpstr url="http://curl.haxx.se/examplepost.cgi"
str file.searchpath("$desktop$\test.txt"); if(!file.len) ret

#compile "CU_def"
CURL curl
int r
HttpPost* formpost=0
HttpPost* lastptr=0
curl_slist* headerlist=0
lpstr buf = "Expect:"

/* Fill in the file upload field */
curl_formadd(&formpost &lastptr CURLFORM_COPYNAME "sendfile" CURLFORM_FILE file CURLFORM_END)
/* Fill in the filename field */
curl_formadd(&formpost &lastptr CURLFORM_COPYNAME "filename" CURLFORM_COPYCONTENTS file CURLFORM_END)
/* Fill in the submit field too, even if this is rarely needed */
curl_formadd(&formpost &lastptr CURLFORM_COPYNAME "submit" CURLFORM_COPYCONTENTS "send" CURLFORM_END)

curl = curl_easy_init(); if(!curl) ret
headerlist = curl_slist_append(headerlist buf)
/* what URL that receives this POST */
curl_easy_setopt(curl CURLOPT_URL url)
 if ( (argc == 2) && (!strcmp(argv[1], "noexpectheader")) )
  /* only disable 100-continue header if explicitly requested */
  curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headerlist);
curl_easy_setopt(curl CURLOPT_HTTPPOST formpost)
r = curl_easy_perform(curl)

curl_easy_cleanup(curl)
curl_formfree(formpost)
curl_slist_free_all(headerlist)
