 /CHI_index_tools

str sm s ss

sm.getmacro(getopt(itemid))
sm.get(sm find(sm "[]#ret")+8)

lpstr s1 s2 s3; int i
m_curfile=0
foreach s sm
	int n=tok(s &s1 3 " ''" 7)
	___CHI_FILE& f=m_af_tools[]
	f.filename=s1; m_curfilename=f.filename
	f.title=s2
	 out "--- %s ---" f.filename
	 index most important keywords. They are specified explicitly.
	if(n=3) ss.from(f.title " " s3); i=findc(ss ';' f.title.len); if(i>0) s=ss+i; ss.fix(i); else s.fix(0)
	else ss=s2; s.fix(0)
	 out "%s, %s, %s, %s" f.filename f.title ss s
	IndexText(m_mw_tools ss 40) ;;most important
	if(s.len) IndexText(m_mw_tools s 10) ;;less important
	 index all strings in these functions
	_s.gett(f.filename)
	sub.GetFunctionStrings(_s s ss)
	 out s
	if(s.len) IndexText(m_mw_tools s 10) ;;actions
	IndexText(m_mw_tools ss 1) ;;all
	 
	m_curfile+1

SaveLoad(1 2)


#sub GetFunctionStrings
function $fname str&sActions str&sAll

str s.getmacro(fname) f2 sac2 sal2

findrx s "\bShowDialog\(''(.+?)''" 0 0 f2 1

ARRAY(str) a; int i
lpstr rx=
 "[a-z\d()\-\.,;'& ]*(?:\[\][a-z\d()\-\.,;'& ]+)+"
findrx s rx 0 5 a
sActions.fix(0); for(i 0 a.len) sActions+a[0 i]

sAll=s
sAll.replacerx("[^'']+?(''.*?''|$|=[](?: [^[]]*[])+)" "$1")

if(f2.len and !(f2=fname))
	sub.GetFunctionStrings(f2 sac2 sal2)
	sActions+sac2
	sAll+sal2


#ret
TO_Text "Text" paste clipboard type keyboard key; enter file display format date time string
TO_Keys "Keys" keyboard press type keystroke enter; text string QM code virtual shortcut release send
TO_Mouse "Mouse" click press left right middle double doubleclick button move pointer cursor mou lef rig mid dou; down up release
TO_Wait "Wait" for pause delay speed; window key mouse web pixel color cursor program cpu processor variable
TO_FindWindow "Find window or control" handle hwnd win child id capture
TO_Window "Window/control actions"; win
TO_Controls "Control-specific actions" child id button check checkbox combo combobox listbox tab; list box item listview status bar
EA_Main "Find accessible object, wait" control acc; for web page internet explorer browser capture
TO_Accessible "Accessible object actions" control; acc web page internet explorer browser enter text click mouse
EH_Main "Find html element, wait" control web page internet explorer browser htm; for capture
TO_Html "Html element actions" control web page internet explorer browser; htm click mouse enter
TO_Menu "Click menu" select men
TO_Scan "Find image, wait" screen scan; for mouse click picture capture
TO_WindowText "Window text" capture scrape; get find wait for mouse click
TO_FileRun "Run program" file execute executable exe
TO_FileRun(1) "Open file" document run
TO_FileRun(2) "Open folder" explore run
TO_FileMkDir "Create folder" new directory mkdir; make
TO_FileCopy "Copy file" folder cop
TO_FileCopy(1) "Move or rename file" folder ren
TO_FileDelete "Delete file" del; recycle
TO_FileIf "If file exists" find search folder directory FileExists searchpath expandpath; special environment full path
TO_FileInfo "Get file info" folder directory size attributes date time find dir; properties information
TO_FileDir "Enumerate files" folder directory list subfolder find dir; all for each wildcard repeat loop
TO_Web "Open web page" wait url browser internet explorer; for run
TO_SendMail "Send email message" mail sendmail; account
TO_ReceiveMail "Receive email messages" mail receivemail; get retrieve account
TO_Mes "Message box" text mes; show dialog
TO_Input "Input box" variable text inp; show dialog user enter
TO_Password "Password input box" inpp; show dialog user enter
TO_List "List box" select; show dialog menu user text item
TO_AutoPassword "Enter password" username login userid autopassword; form dialog name id user
TO_Sound "Sound" play speak audio music bee; file text
New_Dialog "Custom dialog" editor showdialog; show new create user edit
New_Class "New class" wizard; new create user defined member function type constructor destructor base inherit variable
TO_FavMenu "Favorite dialogs" favourite; tools
EW_Main "Explore windows"; process thread view list control menu item
ImageListEditor "ImageList Editor"; icon picture common control list tree view listview treeview
TO_RemapKeyboardKeys "Remap keys" keyboard; change map disable caps lock capslock
