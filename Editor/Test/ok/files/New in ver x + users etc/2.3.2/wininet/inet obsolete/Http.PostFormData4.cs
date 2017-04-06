 /
function# $action [ARRAY(POSTFIELD)&a] [str&responsepage] [$headers] [fa] [fparam] [inetflags] [str&responseheaders] [flags] ;;flags: 16 download to file, 32 run in other thread

 Posts web form data.
 Returns 1 if successful, 0 if fails.
 At first call Connect to connect to web server.
 This function is similar to Post, but has more features. Can post files.
 QM 2.3.2. If fails to open file (for sending), returns 0. In earlier versions would throw error.

 action - script's path relative to server. See "action" field in form's HTML. Example "forum\login.php".
 a - array containing data.
   QM 2.3.2. Can be 0. Then uses data added with <tip>Http.PostAdd</tip>.
   POSTFIELD members:
      name - field name. Same as "name" field in form's HTML.
      value - field value. Same as "value" field in form's HTML. If it is file field, must specify file.
      isfile - must be 1 for file fields, 0 for other fields.
 responsepage - receives response page (HTML).
 headers - additional headers to send.
 fa - address of user-defined callback function. It can be used to show progress.
   function# action nbAll nbWritten $_file nbAllF nbWrittenF fparam
   Arguments:
     action - 0 before posting, 1 sending file, 2 all data sent, 3 starting to receive response, 4 finished.
     nbAll - total size of data being uploaded.
     nbWritten - size of already uploaded part of total.
     _file - current file.
     nbAllF - size of current file.
     nbWrittenF - size of already uploaded part of current file.
   Return value: 0 continue, 1 cancel.
   Example: search in forum.
   The function is not called while downloading response. If need, use SetProgressCallback to set another function for it.
 fparam - some value to pass to the callback function.
 inetflags (QM 2.2.1) - INTERNET_FLAG_x flags. Documented in MSDN library. For example, use INTERNET_FLAG_NO_AUTO_REDIRECT to disable redirection. Flag INTERNET_FLAG_RELOAD is always added.
 responseheaders (QM 2.2.1) - receives raw response headers.
 flags (QM 2.3.2):
    16 - download to file. responsepage must contain file path.
    32 - run in other thread. Then current thread can receive messages, and therefore this function can be used in a dialog.

 EXAMPLE
 ARRAY(POSTFIELD) a.create(2)
 a[0].name="testtxt"; a[0].value="some text"; a[0].isfile=0
 a[1].name="testfile"; a[1].value="$desktop$\test.gif"; a[1].isfile=1
 Http h.Connect("www.xxx.com"); str r
 if(!h.PostFormData("form.php" a r)) end "failed"
 out r


int r i postFiles
str s s1 s2

if(!&a) &a=m_ap

for i 0 a.len
	if(!a[i].name.len) end "field name is empty"
	if(a[i].isfile) postFiles=1

if postFiles or fa
	r=PostFD(action &a responsepage headers fa fparam inetflags responseheaders flags)
else ;;urlencode and call Post
	for i 0 a.len
		s1=a[i].name; s2=a[i].value
		s.formata("%s%s=%s" iif(i "&" "") s1.escape(9) s2.escape(9))
	r=Post(action s responsepage headers inetflags responseheaders flags)

if(&a=&m_ap) a=0
ret r
err+ end _error
