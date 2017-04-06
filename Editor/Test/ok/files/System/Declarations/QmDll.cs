def WM_QM_ENDTHREAD WM_APP+0x2200
def WM_QM_DRAGDROP WM_APP+0x2201
type QMDRAGDROPINFO hwndTarget IDataObject'dataObj ARRAY(FORMATETC)formats keyState POINT'pt effect str'files 
type SHORTCUTINFO ~target ~param ~initdir ~descr ~iconfile iconindex showstate @hotkey
type QMTHREAD qmitemid threadid threadhandle flags tuid
type __RTO_OPT !keysync !keymark !keychar !hungwindow !clip
type RTOPTIONS
	flags spe_for_macros waitcpu_time waitcpu_threshold
	str'web_browser_class str'net_clr_version
	__RTO_OPT'opt_macro_function __RTO_OPT'opt_menu_toolbar __RTO_OPT'opt_autotext

dll "qm.exe"
	!*q_malloc size ;;QM malloc
	!*q_calloc elemcount elemsize ;;QM calloc
	!*q_realloc !*mem size ;;QM realloc
	q_free !*mem ;;QM free
	#q_msize !*mem ;;QM _msize
	$q_strdup $s ;;QM _strdup
	#PidlToStr ITEMIDLIST*pidl str&s [flags] ;;Returns 1 if successful.  flags: 1 must be path, 2 must be ":: ITEMIDLIST", 4 path can be URL, 8 path can be "::{CLSID}", 16 path must be display name
	ITEMIDLIST*PidlFromStr $s
	#GetFileIcon $_file [index] [flags] ;;Returns icon or cursor handle if successful. Later call DestroyIcon or DestroyCursor.  flags: 1 large, 2 don't use shell icon, 4 cursor, 8 create empty if fails, hiword size
	#CreateShortcutEx $shortcut SHORTCUTINFO&si ;;Returns 1 if successful.
	#GetShortcutInfoEx $shortcut SHORTCUTINFO&si ;;Returns 1 if successful.
	#SaveBitmap hbitmap $_file [flags] ;;Returns 1 if successful.  flags: 1 _file is IStream.
	#LoadPictureFile $_file [flags] ;;Returns bitmap handle.  flags: 1 return IPicture
	#Crc32 !*data nBytes [flags] ;;Returns CRC of data. If data is string, nBytes can be -1.  flags: 1 data is file (nBytes not used).
	#RichEditLoad hwndRE $_file ;;Returns 1 if successful.
	#RichEditSave hwndRE $_file ;;Returns 1 if successful.
	#QmKeyCodeToVK $qmkey int&vk ;;Returns number of characters eaten.
	QmKeyCodeFromVK vk str&qmkey
	FormatKeyString !vk !mod str&s ;;mod: 1 Shift, 2 Ctrl, 4 Alt, 8 Win.
	#GetMod ;;Returns: 1 Shift, 2 Ctrl, 4 Alt, 8 Win.
	#GetCPU ;;Returns % CPU usage, 0 to 100.
	#GetDiskUsage ;;Returns % disk usage, 0 to 100.
	#IsThreadRunning $threadName ;;Returns number of threads.  name can be QM item name or +id.
	#EnumQmThreads [QMTHREAD*arr] [nElem] [flags] [$threadName] ;;flags: must be 0
	#GetQmThreadInfo handle QMTHREAD&qt ;;Returns 1 on success, 0 on failure.  handle=0 - current thread.
	#SetPrivilege $privilege ;;Returns 1 if successful.
	[HtmlHelpU]#HtmlHelp hwndCaller $pszFile uCommand dwData ;;HtmlHelp that supports UTF-8. For reference, search for HtmlHelp in MSDN Library.
	#IsLoggedOn ;;Returns: 0 not logged on, 1 normal, 2 locked, 3 switched off, 4 custom desktop.
	#EnsureLoggedOn [unlock] ;;Returns: 0 cannot ensure, 1 normal, 2 successfully unlocked.
	#AllowActivateWindows ;;Returns 1 if successful
	EnumProcessesEx ARRAY(int)&pids [ARRAY(str)&names] [flags] ;;flags: 1 full path, 2 current user session only
	#ProcessNameToId $exename [ARRAY(int)&allPids] [flags] ;;allPids can be 0;  flags: 1 current user session only
	#ProcessHandleToId hProcess
	#GetExeResHandle
	%GetFileOrFolderSize $_file
	#IsWindow64Bit hwnd [flags] ;;flags: 1 hwnd is process id
	#RegisterComComponent $path [flags] ;;flags: 1 unregister, 2 check extension (must be dll or ocx), 4 run as admin, 8 show error/success message box
	#GetProcessUacInfo hwnd [flags] ;;returns: 0 XP or failed, 1 UAC off or non-admin account, 2 admin, 3 uiAccess, 4 user;  flags: 1 hwnd is process id (0 = current process)
	#GetProcessIntegrityLevel hwnd [flags] ;;returns: 0 XP or failed, 1 Low, 2 Medium, 3 High, 4 System;  flags: 1 hwnd is process id (0 = current process)
	#QmRegisterDropTarget hwndTarget hwndNotify flags ;;flags: 1 notify on enter, 2 notify on over, 4 notify on leave, 16 need only files, 32 need .lnk
	#QmUnregisterDropTarget hwndTarget
	#IsUserAdmin ;;Returns 1 if this process (QM or exe) is running as administrator
	#GetToolbarOwner hwndTb ;;Returns handle of a QM toolbar's owner window
	#ExeExtractFile resId $dest [flags] [resType] ;;Returns: 1 success, 0 failed.  flags: 1 load dll and return handle
	#ExeGetResourceData resType resId str&data [int&pSize] ;;Returns: 1 success, 0 failed.  resType: if 0, uses RT_RCDATA. If data 0, returns pointer to resource memory.
	#RealGetKeyState vk ;;Returns 1 if the key is pressed. If vk contains flag 0x100 - if toggled. vk is virtual-key code.
	^Round ^number [cDec] ;;Returns number rounded to cDec digits after decimal point
	^RandomNumber ;;Returns random number between 0.0 and 1.0, not including them
	#RandomInt bound1 bound2 ;;Returns random number between bound1 and bound2, including them
	AdjustWindowPos hwnd RECT&r [flags] [monitor] ;;flags: 1 work area, 2 actually move, 4 raw x y, 8 only move into screen, 16 can resize, 32 monitor is hmonitor.  monitor: 0 primary, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle
	#MonitorFromIndex [monitor] [flags] [RECT&r] ;;flags: 1 work area, 32 monitor is hmonitor.  monitor: 0 primary, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle
	#MonitorIndex hmonitor ;;Returns 1-based monitor index.
	#GetProcessExename pid str&exename [flags] ;;flags: 1 full path.  Returns: 1 success, 0 failed.
	MenuSetString hMenu item $s ;;item: > 0 id, <=0 -position
	#MenuGetString hMenu item str&s ;;item: > 0 id, <=0 -position.   Returns: 1 success, 0 failed.
	EditReplaceSel hDlg cid $s [flags] ;;flags: 1 replace all, 2 set focus, 4 move caret to end
	#GetUserInfo str&s [flags] ;;flags: 0 user name, 1 computer name
	GetFullPath $sAny str&sFull
	SetWinStyle hwnd style [flags] ;;flags: 0 set, 1 add, 2 remove, 4 exstyle, 8 update nonclient, 16 update client
	[q_isdigit]#isdigit char
	[q_isalnum]#isalnum char
	[q_iscsym]#iscsym char ;;ASCII letter, digit or _
	#StrCompare $s1 $s2 [insens]
	#StrCompareN $s1 $s2 n [insens]
	#MemCompare !*s1 !*s2 n [insens]
	#q_stricmp $s1 $s2
	#q_strnicmp $s1 $s2 count
	#q_memicmp $s1 $s2 count
	q_printf $format ...
	#q_sort !*base num width func [!*param]
	%ConvertSignedUnsigned int'value [valueType] ;;valueType: 0 unsigned int, 1 signed byte (char), 2 signed word (short)
	#DetectStringEncoding $s ;;Returns: 0 ASCII, 1 UTF8, 2 other.
	#StrCompareEx $s1 $s2 compare ;;compare: 0 simple, 1 insens, 2 ling, 3 ling/insens, 4 number/ling/insens, 128 date
	#QmSetWindowClassFlags $cls flags ;;flags: 0x80000000 get, 1 use dialog variables, 2 use dialog definition text, 4 disable text editing in Dialog Editor, 8 supports "WM_QM_DIALOGCONTROLDATA"
	RtOptions mask RTOPTIONS&x ;;mask: 0 get all, 1 flags, 2 spe_for_macros, 4 waitcpu_time, 8 waitcpu_threshold, 16 web_browser_class, 32 net_clr_version, 64 opt_
	PerfFirst
	PerfNext
	PerfOut [flags] [str&sOut] ;;flags: 1 begin collect, 2 collect, 3 out collected
	#IsValidCallback addr nBytesParam
	#Statement [caller] [statOffset] [str&statement] [int&itemId] [flags] ;;flags: 1 include indirect callers
	GetCallStack [str&cs] [flags] ;;flags: 1 no code, 2 no formatting
	#DpiIsWindowScaled hwnd ;;if hwnd 1, gets is scaling enabled
	#DpiGetWindowRect hwnd RECT*r [flags] ;;flags: 4 client, 8 client in screen
	#DpiClientToScreen hwnd POINT*p [flags] ;;flags: 16 window, 0x100 RECT
	#DpiScreenToClient hwnd POINT*p [flags] ;;flags: 16 window, 0x100 RECT
	DpiMapWindowPoints hwnd1 hwnd2 POINT*p n [flags]
	DpiScale POINT*p n ;;n: >0 scale, <0 unscale
	#DpiGetDPI
	#FileExists $_file [flags] ;;flags: 0 file, 1 folder, 2 file or folder.   Returns: 0 not found, 1 file, 2 folder.
	RedirectQmOutput redirFunc
	[__WinTest]#WinTest hwnd $cls ;;Returns 1 if window class is cls. Supports wildcard *?. Can be list of classes like "Class1[]Class2", then returns 1-based index of matching class.

 not in exe
dll? "qm.exe"
	CompileAllItems $folder [$excludeList]
	#SilentImport $_file [flags] ;;_file: qml file to import; flags: 1 add shared, 2 at bottom.   Returns: 1 success, 0 failed.
	#SilentExport $item $_file [flags] ;;item: qm item or folder name or id;  _file: folder or qml file or "";  flags: 1 no import, 2 no open, 4 no shared, 8 no partial/renamed, 16 read-only, 0x100 make zip.   Returns: 1 success, 0 failed.
	#RecGetWindowName hwnd str&s [flags] [str&sParent] [str&sComments] [str&sComments2]
	GetLastSelectedMenuItem str&label [str&command] [flags] ;;flags: 1 get pidl always
	#StartProcess flags $path [$cl] [$defDir] [int&hProcess] ;;flags: 0 as QM, 1 medium, 2 high, 3 high with consent, 4 low, 5 as QM but not uiAccess, 16 get pid, hibyte showcmd
	InsertStatement $s [$label] [$icon] [flags] ;;flags: 1 simple, 2 set focus, 4 don't indent
	#GetQmCodeEditor
	$OutWinMsg message wParam lParam [str&sOut] [hwnd]
	#UnloadDll $dllName
	QmCodeToHtml $sIn str&sOut itemId bbcode flags
	#GetQmItemsInFolder $folder ARRAY(QMITEMIDLEVEL)&a [flags] ;;1 only direct children

 undocumented or obsolete. Should be used only in System. Some may be changed or deleted in the future.
#opt hidedecl 1
dll "qm.exe"
	#__CompileDialogDefinition $sdd str&so cbfa cbparam
	#__CDD_CanHaveText $cls style
	#__CDD_LoadControlDllOnDemand $cls
	#DialogBoxIndirectParamU modeless DLGTEMPLATEEX*hDialogTemplate hWndParent lpDialogFunc dwInitParam
	#FindFirstFileU $lpFileName WIN32_FIND_DATAU*lpFindFileData
	#FindNextFileU hFindFile WIN32_FIND_DATAU*lpFindFileData
	[ProcessNameToId]#ProcessNameToId2 $exename [ARRAY(int)&allpids] [flags] ;;obsolete. Use ProcessNameToId.
	#__GetQmItemIcon $name [flags] ;;flags: 1 big, hiword custom size
	#__ImageListLoad $_file [flags] ;;flags: 1 IStream, 2 _file is list of icon files (HIWORD can be icon size, default 16)
	[SelStr]#__SelStr flags $name $s1 ...
	[q_iscsym]#__iscsym char ;;same as iscsym
	__VarProp_Set !*varAddr value
	#__VarProp_Remove !*varAddr
	#__VarProp_Get !*varAddr [int&found]
	#__VarProp_GetVar value [int&found]
	INTLPSTR*__SfMap *n
	#__Val $s [$&_se] [flags] [`&v] ;;flags: 0 dec/hex, 1 +float, 2 all (free format), 4 no hex, 8 only hex (with or without 0x), 8|4 only hex without 0x
	#RealGetParent hWnd
	#SubclassWindow hwnd newWndProc ;;obsolete, use SetWindowSubclass
	SECURITY_ATTRIBUTES*GetSecurityAttributes
	#InitWindowsDll what ;;what: 0 winsock, 1 GDI+
	[InitWindowsDll]#__WsInit [__] ;;fbc
	#CreateFileU $lpFileName dwDesiredAccess dwShareMode SECURITY_ATTRIBUTES*lpSecurityAttributes dwCreationDisposition dwFlagsAndAttributes hTemplateFile
	#__TrackPopupMenu hMenu [hwndOwner] [tpmFlags] [POINT*p]
	#WaitForAnActiveWindow waitMS [flags] ;;flags: 1 focused control, 2 always process messages
	__SetAutoSizeControls hDlg $controls
	#__SysLinkOnClick hWnd NMHDR*nmlink
	$__Str_GetNotThis str*_this lpstr*s str*sStore
	[Brac2]$__Brac2 $s lc [rc] [tokFlags]

 undocumented, not in exe
dll? "qm.exe"
	#__TypeLibDialog hwndOwner str&getActiveX
	#__BitmapToIcons $bmpfile $outputFolder maskColor flags ;;maskColor: -1 top-left pixel, -2 no mask.  flags: 1 open folder
	#__LocalVarUniqueName $name str&uniqueName [$moreVars] [$_type]
	#__LocalVarGetAll str&sVar [str&sText]
	__QmKeysToText $s str&so
	#__LenId $s [flags] ;;If s begins with a valid identifier word, returns its length, else 0.  flags: 1 any word, ie can beging with a digit and be of any length.
	#__Eval $s ;;Calculates integer value of expression with one or more integer numbers, constants and operators. On error returns 0, and GetLastError returns ERROR_INVALID_PARAMETER.
	#__Tok $s ARRAY(str)&arr [$_types]
	#__RecIsPointInNonclient hwnd x y
	#__TriggerInfoAutotext str&userText int&postfixChar int&prefixChar str&itemText int&itemFlags
	#__CallbackGetAddress iid
	__ResourcesDialog hwndCallback msgCallback
	#__QmHelp action $helpId [$anchor]
	__PlayQmSound sound
	#dlg_TbGetDimensions htb idButton what ;;what: 0 top, 1 right, 2 bottom, 3 left, 4 width, 5 height, 6 real toolbar width, 7 height of buttons, 8 width of buttons, 9 MAKELONG(8, 7)
	__ScheduleUpdated

 from qmplus.dll
dll- qmplus
	qmplus_str_wrap str&s width $repl $delim _flags minWidth
	qmplus_str_stem str&s stemflags from wordLen
 note: not "$qm$\qmplus", because will auto-add to exe

#opt hidedecl 0


interface# IStringMap :IUnknown
	[p]Flags(flags) ;;1 case insens., 2 exists - do nothing, 4 exists - replace, 8 exists - add new, 16 exists - compare
	[g]#Flags()
	Add($k [$v])
	AddList($s [$sep]) ;;sep: separator character, like "=", default " ", can be "csv"
	$Get($k)
	$Get2($k str&v)
	Set($k $v)
	Rename($k $newname)
	Remove($k)
	RemoveAll()
	[g]#Count()
	GetAll(ARRAY(str)&ak [ARRAY(str)&av]) ;;ak or av can be 0
	#GetAllOf($k ARRAY(str)&av)
	GetList(str&s [$sep]) ;;sep default is " "
	EnumBegin()
	#EnumNext(str&k [str&v])
	EnumEnd()
	IntAdd($k v)
	#IntGet($k int&v)
	IntSet($k v)
	{3426CF3C-F7C1-4322-A292-463DB8729B53}
dll "qm.exe" IStringMap'CreateStringMap [flags] ;;flags: 1 case insens., 2 exists - do nothing, 4 exists - replace, 8 exists - add new, 16 exists - compare

interface# ICsv :IUnknown
	[p]Separator($sep)
	[g]$Separator()
	FromString($s)
	ToString(str&so)
	FromFile($_file)
	ToFile($_file [flags]) ;;flags: 1 append, 0x100 safe, 0x200 safe+backup
	FromArray(ARRAY(str)&a)
	ToArray(ARRAY(str)&a)
	FromQmGrid(hwnd [flags]) ;;flags: 1 no first column, 2 no empty rows, 4 selected/checked, 8 remove <...>
	ToQmGrid(hwnd [flags]) ;;flags: 1 only first column, 2 no first column
	Clear()
	[g]#RowCount()
	[g]#ColumnCount()
	[p]ColumnCount(count)
	[g]$Cell(row col)
	[p]Cell(row col $value)
	[g]#CellInt(row col)
	[p]CellInt(row col value)
	[p]CellHex(row col value)
	RemoveRow(row)
	#AddRowMS(row [nCells] [$cells] [firstCell]) ;;Adds or inserts new row from multistring. Use row -1 to add to the end. Returns row index.
	#AddRowLA(row [nCells] [lpstr*cells] [firstCell]) ;;Adds or inserts new row from array of lpstr values. Use row -1 to add to the end. Returns row index.
	#AddRowSA(row [nCells] [str*cells] [firstCell]) ;;Adds or inserts new row from array of str values. Use row -1 to add to the end. Returns row index.
	#AddRow1(row $s1) ;;Adds or inserts new row in 1-column CSV. Use row -1 to add to the end. Returns row index.
	#AddRow2(row $s1 [$s2]) ;;Adds or inserts new row in 2-column CSV. Use row -1 to add to the end. Returns row index.
	#AddRow3(row $s1 [$s2] [$s3]) ;;Adds or inserts new row in 3-column CSV. Use row -1 to add to the end. Returns row index.
	#AddRowCSV(row $csv) ;;Adds or inserts one or more rows from CSV string. Use row -1 to add to the end. Returns first new row index.
	#ReplaceRowCSV(row $csv) ;;Replaces one or more rows from CSV string. Returns first replaced row index.
	#ReplaceRowMS(row [nCells] [$cells] [firstCell])
	#ReplaceRowLA(row [nCells] [lpstr*cells] [firstCell])
	#ReplaceRowSA(row [nCells] [str*cells] [firstCell])
	GetRowMS(row str*cells)
	MoveRow(row to)
	InsertColumn(col)
	RemoveColumn(col)
	[p]RowDataSize(nBytes)
	[g]#RowDataSize()
	[g]!*RowData(row)
	Sort(flags [col]) ;;flags: 0 simple, 1 insens, 2 ling, 3 ling/insens, 4 number/ling/insens, 128 date, 0x100 descending
	#Find($s [flags] [col] [startRow]) ;;flags: 1 insens, 2 wildcard, 4 beginning, 8 end, 16 middle, 32 rx
	{3426CF3C-F7C1-4322-A292-463DB8729B54}
dll "qm.exe" ICsv'CreateCsv [flags] ;;flags: 1 separator is comma.  obsolete, use _create.

type XMLNODE $name $value @level !xtype !flags userdata

interface# IXmlNode :IUnknown
	[g]IUnknown'XmlDoc()
	[g]IXmlNode'Parent()
	[g]IXmlNode'Prev()
	[g]IXmlNode'Next()
	[g]IXmlNode'FirstChild()
	[g]IXmlNode'LastChild()
	[g]IXmlNode'Child($name [index])
	[g]IXmlNode'Attribute($name)
	IXmlNode'Path($path [ARRAY(IXmlNode)&allMatching] [flags]) ;;flags: 1 search all paths, 2 * matches only elements.    path examples: "node",  "elem/node",  "elem/@a",  "*", "elem/*",  "elem/@*",  "*/node",  "../node",  ".//node",  "elem[='abc']",  "*[='abc']",  "elem[@attr='abc']",  "elem[@attr*='ab*']",  "elem[child='abc']",  "elem[child]"
	[g]$Name()
	[p]Name($name)
	[g]$Value()
	[p]Value($value)
	ValueBinaryGet(str&value)
	ValueBinarySet(str&value [flags]) ;;flags: 1 compress, 2 hex
	[g]#Type()
	[g]#UserData()
	[p]UserData(userdata)
	Properties(XMLNODE&xi)
	[g]$ChildValue($name [index])
	[g]$AttributeValue($name)
	[g]#AttributeValueInt($name)
	IXmlNode'Add($name [$value])
	IXmlNode'Insert(IXmlNode'iafter $name [$value])
	IXmlNode'SetChild($name $value)
	IXmlNode'SetAttribute($name $value)
	IXmlNode'SetAttributeInt($name value)
	Move(IXmlNode'parent IXmlNode'iafter)
	GetAll(flags ARRAY(IXmlNode)&a) ;;flags: 1 +attributes, 2 only direct children
	[g]#ChildCount()
	DeleteChild($name)
	DeleteAttribute($name)
	{3426CF3C-F7C1-4322-A292-463DB8729B56}

interface# IXml :IUnknown
	[p]Flags(flags) ;;1 normalize newlines, 2 enable UserData, 4 ignore encoding, 8 auto save, 0x100 safe save, 0x200 safe save+backup
	[g]#Flags()
	[g]IXmlNode'Root()
	[g]IXmlNode'RootElement()
	IXmlNode'Path($path [ARRAY(IXmlNode)&allMatching] [flags]) ;;flags: 1 search all paths, 2 * matches only elements.    path examples: "node",  "elem/node",  "elem/@a",  "*", "elem/*",  "elem/@*",  "*/node",  "../node",  ".//node",  "elem[='abc']",  "*[='abc']",  "elem[@attr='abc']",  "elem[@attr*='ab*']",  "elem[child='abc']",  "elem[child]"
	IXmlNode'Add($name [$value])
	Delete(IXmlNode&node)
	Clear()
	[g]$XmlParsingError
	[g]#Count()
	IXmlNode'FromString($s)
	ToString(str&so)
	IXmlNode'FromFile($_file [$defaultXML])
	ToFile([$_file] [flags]) ;;flags: 0x100 safe, 0x200 safe+backup
	{3426CF3C-F7C1-4322-A292-463DB8729B55}
dll "qm.exe" IXml'CreateXml [flags] ;;flags: 1 normalize newlines, 2 enable UserData, 4 ignore encoding, 8 auto save, 0x100 safe save, 0x200 safe save+backup

 xml element types
def XT_Root 0
def XT_Element 1
def XT_Attribute 2
def XT_Text 3
def XT_XmlDeclaration 4
def XT_DocumentType 5
def XT_ProcessingInstruction 6
def XT_CDATA 7
def XT_Comment 8
 xml element flags
def XF_HasText 1
def XF_HasChildNodes 2
def XF_HasAttributes 4
def XF_XmlDeclarationAttribute 128
 CreateXml flags
def XO_NormalizeNewlines 1
def XO_HasUserdata 2
def XO_IgnoreEncoding 4
def XO_AutoSave 8
def XO_SafeSave 0x100
def XO_SafeSaveBackup 0x200

 private, not documented
interface# __IImageLibrary :IUnknown
	CreateNew(imageSize $xmlFile [$ilFile])
	Load($xmlfile)
	Save([flags])
	#GetImageList
	#AddIcon($iconfile)
	ReplaceIcon(imageindex $iconfile)
	InsertIcon(imageindex $iconfile)
	RemoveIcon(imageindex)
	#RefreshIcons([flags] [iFrom] [n])
	[g]#ImageIndex($imagefile)
	[g]$ImagePath(imageindex)
	[g]#Count()
	[p]Attribute($name $value)
	[g]$Attribute($name)
	SetFlags(flags)
dll? "qm.exe" [CreateImageLibrary]__IImageLibrary'__CreateImageLibrary

 single instance - _qmfile
interface# __IQmFile :IUnknown
	SettingAddB($macro $settName !*data nBytes)
	SettingAddS($macro $settName $data)
	SettingAddI($macro $settName data)
	SettingGetB($macro $settName !*data nBytes)
	$SettingGetS($macro $settName [str&data])
	#SettingGetI($macro $settName)
	SettingDelete($macro $settName)
	ResourceAdd($macro $resName !*data nBytes)
	ResourceGet($macro $resName str&data [flags])
	ResourceDelete($macro $resNameWildcard)
	ResourceRename($macro $oldName $newName)
	ResourceEnum($macro $resNameWildcard ARRAY(str)&aName [ARRAY(str)&aData])
	!*SqliteBegin([iid])
	SqliteEnd()
	#SqliteItemProp($macro [int&rowid] [GUID&guid])
	FullSave()
	GetLoadedFiles(ARRAY(str)&aFiles)
dll? "qm.exe" __IQmFile'__GetQmFile

#opt hidedecl 1
interface# IQmDropdown :IUnknown
	Check(i flags)
	#IsChecked(i)
	#Select(i [flags])
	Close([flags])
	Update(iFirst [iLast] [flags])
	[g]#Hwnd()
	[g]#HwndLV()
	[g]#SelectedItem()
	[g]ARRAY(byte)ItemStates()
	[g]ICsv'Csv()
	{A461B246-3C33-4398-915B-0B834A60A670}
#opt hidedecl 0
dll- "$qm$\qmgrid.dll"
	#ShowDropdownList ICsv'csv [int&iSelected] [ARRAY(byte)&aChecked] [funcFlags] [hwndHost] [RECT&rDD] [cbFunc] [cbParam] [IQmDropdown&dd]
	#ShowDropdownListSimple $csv [int&iSelected] [ARRAY(byte)&aChecked] [funcFlags] [hwndHost] [RECT&rDD]

 ShowDropdownList and/or QM_ComboBox control
 CSV flags (cell 0 2). Used with ShowDropdownList and QM_ComboBox.
def QMDD_CHECKBOXES1 1
def QMDD_CHECKBOXES2 2
def QMDD_CHECKBOXES3 3
def QMDD_NOMOUSECLOSE 4
def QMDD_AUTOCOLORS 8
def QMDD_MULTICOLUMN 0x10
def QMDD_MULTICOLUMN_HORZ 0x20
def QMDD_WIDTHUNLIMITED 0x40
def QMDD_FASTDD 0x80
 these used only with QM_ComboBox. CSV cell 0 2 too.
def QMCB_GETCSV 0x100
def QMCB_NOCLICKSELECT 0x200
def QMCB_NOCLEAREDIT 0x400
def QMCB_TEXTOROROR 0x800
def QMCB_AUTOADDITEM 0x1000
 CSV item flags. Used with ShowDropdownList and QM_ComboBox.
def QMDDI_CHECKED 1
def QMDDI_NOCHECK 2
def QMDDI_DISABLED 4
def QMDDI_BOLDFONT 8
def QMDDI_SELECTABLECHECK 16
def QMDDI_CHECKGROUP 32
def QMDDI_CHECKSTATECHANGED 0x80 ;;used for output only
 ShowDropdownList funcFlags
def QMDDFF_CSVINPUT 1
def QMDDFF_SORT 8
 currently undocumented:
def QMDDFF_CALLBACKONMOUSEMOVE 0x10 ;;on wm_mousemove call cbFunc with code=10
def QMDDFF_ASSUBMENU 0x20 ;;showed on WM_INITMENUPOPUP and must work more like a submenu; rRect is submenu-item rect
 ShowDropdownList return flags
def QMDDRET_SELOK 1
def QMDDRET_CHECKCHANGED 2
def QMDDRET_SELCHANGED 4
def QMDDRET_NOERRORS 0x100
 callback data
type QMDROPDOWNCALLBACKDATA IQmDropdown'dd code itemIndex itemState
type NMQMCB NMHDR'hdr IQmDropdown'dd itemIndex itemState

 QM_Edit control
def QMEM_SETBUTTON 0x500
