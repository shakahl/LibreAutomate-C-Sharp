 1. In QM.

 get selected item in qm items list
int htv=id(2202 _hwndqm)
int iid=TvGetParam(htv SendMessage(htv TVM_GETNEXTITEM TVGN_CARET 0))

 is it folder?
_s.getmacro(iid 3)
if(_s!=5) ;;not folder
	mes "This macro attaches a QM folder to the post in the forum. Please select the folder and run this macro again.[][]If you want to post a single macro, instead use menu Edit -> Other Formats."
	ret

 temporary file path
str name.getmacro(iid 1)
str tfile.from("$temp qm$\export to forum\" _s.RandomString(10 10 "qwertyuiopasdfghjkl") "\" name ".qml") ;;why we use random folder? because FF does not close the file, and then next time SilenExport fails.
tfile.expandpath
 out tfile;; ret

 export the folder to the file
del "$temp qm$\export to forum"; err
if(!SilentExport(+iid tfile 2)) mes- "failed to export the folder"

 out items
str items
GetQmFolderItems +iid items
out "FUNCTIONS:[]%s" items

 out word list
str words
GetQmFolderWordList +iid words
out "[size=1]WORD LIST: %s[/size]" words

 ret
 ---------------------

 2. In browser.

 activate browser (firefox or internet explorer)
int ie; Acc a adoc
int hwnd=win("Quick Macros Forum" "Mozilla*WindowClass" "" 0x804)
if(!hwnd) hwnd=win("Quick Macros Forum" "IEFrame"); if(hwnd) ie=1; else goto ge1
act hwnd
0.3

if(ie) adoc=acc("" "PANE" hwnd "Internet Explorer_Server" "" 0x1000)
else adoc=acc("" "DOCUMENT" hwnd "" "" 0x1080)

 click 'Upload attachment'
a=acc("Upload attachment" "LINK" adoc "" "" 0x1011); err goto ge1
a.DoDefaultAction

 type the file path
 a=acc("Browse*" "PUSHBUTTON" a "" "" 0x1091 0 0 "previous")
 a.SetValue(tfile) ;;does not work with file input elements
 a.Select(1) ;;in FF works but I cannot type text; is the edit field readonly?
 a.Mouse(1) ;;also cannot be used in FF because it opens the dialog
 so we have to click Browse...
a=acc(iif(ie "Filename:" "Browse*") "PUSHBUTTON" adoc "" "" 0x1011)

0.5
a.DoDefaultAction
int hfu
if(ie) wait(10 WA win("Choose file*" "#32770" "IEXPLORE" 3))
else hfu=wait(10 WA win("File Upload" "" "FIREFOX" 3))
key (0.3) (tfile) (0.3) Y (0.1)

 click 'Add the file' button
 a.Navigate(iif(ie "next2" "parent next2"))
 a.DoDefaultAction
 disabled the above code to allow you to click either 'Add the file' or 'Update file'

ret
 ge1
mes "Internet Explorer or Firefox must be running, with 'Post a new topic' or similar page in QM forum."
