category mouse :
	">\System\Functions\Mouse" ">\User\Functions\Mouse"
	lef rig mid dou mou xm ym ifk opt
	BlockInput RealGetKeyState

category keytext :;;    keys, text, clipboard. Added in QM 2.3.2.
	">\System\Functions\KeyText" ">\User\Functions\KeyText"
	key paste ifk out opt
	GetMod RealGetKeyState BlockInput
	">\User\Functions\Input-Output"
	"?str getclip getsel setclip setsel"

category dialog :;;    dialogs, popup menu, OSD
	">\System\Functions\Dialog" ">\User\Functions\Dialog"
	mes inp inpp
	ShowDialog FontDialog ExeOutputWindow MenuPopup ShowDropdownList ShowDropdownListSimple
	_monitor

category file :;;    run, files, folders, database, XML, CSV
	">\System\Functions\File" ">\User\Functions\File"
	run cop ren del mkdir zip rget rset
	OpenSaveDialog BrowseForFolder ExeFullPath
	FileExists CreateShortcutEx GetShortcutInfoEx GetFileOrFolderSize AllowActivateWindows GetFullPath
	RichEditLoad RichEditSave RegisterComComponent StartProcess
	Dir Database IXml IXmlNode CreateXml ICsv CreateCsv Sqlite
	Wsh Shell32 SHDocVw ADO
	_logfile _logfilesize _qmdir
	"?str dospath expandpath getfile getfilename getpath searchpath setfile GetFilenameExt ReplaceInvalidFilenameCharacters UniqueFileName DateInFilename LoadUnicodeFile SaveUnicodeFile SqlEscape"
	"?Dir"

category window :;;    windows, controls
	">\System\Functions\Window" ">\User\Functions\Window"
	act clo min max res mov siz hid ifa win wintest opt
	IsWindow IsZoomed IsIconic IsWindowVisible IsWindowEnabled EnableWindow IsHungAppWindow
	MoveWindow SetWindowPos ShowWindow GetWindow GetParent GetAncestor
	GetWindowRect GetClientRect ScreenToClient ClientToScreen MapWindowPoints
	GetProp SetProp RemoveProp GetWindowLong SetWindowLong GetWinId WinTest
	SendMessage SendMessageW SendMessageTimeout SendMessageTimeoutW SendNotifyMessage PostMessage
	AllowActivateWindows AdjustWindowPos IsWindow64Bit SetWinStyle
	GetProcessUacInfo GetProcessIntegrityLevel
	DpiGetDPI DpiIsWindowScaled DpiGetWindowRect DpiClientToScreen DpiScreenToClient DpiMapWindowPoints DpiScale
	_hwndqm _monitor
	"?str getwinclass getwinexe getwintext setwintext"

category control :;;    controls, UI objects, menu, Excel, find image
	">\System\Functions\Control" ">\User\Functions\Control"
	men but id child childtest acc acctest htm act scan pixel
	Acc Htm ExcelSheet WindowText
	"?str getwinclass getwinexe getwintext setwintext"

category time :;;    wait, date/time
	">\System\Functions\Time" ">\User\Functions\Time"
	wait spe tim perf opt
	GetTickCount timeGetTime GetLocalTime SetLocalTime GetSystemTime SetSystemTime FileTimeToSystemTime SystemTimeToFileTime SetTimer KillTimer
	PerfFirst PerfNext PerfOut
	DATE DateTime
	"?str timeformat DateInFilename"
	"?DateTime"

category internet :
	">\System\Functions\Internet" ">\User\Functions\Internet"
	web run htm
	GetIpAddress
	Http Ftp HtmlDoc Pop3Mail SmtpMail MailBee
	_iever

category string :
	">\System\Functions\String" ">\User\Functions\String"
	len empty val numlines find findw findt findl tok findc findcr findcs findcn findb findrx matchw
	isalnum isalpha iscsym isdigit isxdigit isupper toupper islower tolower IsCharLowerW IsCharUpperW CharLowerW CharUpperW
	setlocale strrev StrCompare StrCompareN MemCompare memset memcpy memmove memchr
	lstrcpy lstrcpyn lstrcpyW lstrcpynW lstrlenW
	q_sort DetectStringEncoding FormatKeyString
	CreateStringMap IStringMap HtmlDoc
	_unicode ExeParseCommandLine
	"?str"
	"?(character) {byte ch = s[i]}"
	"?(to utf16) {word* ws = @s}"
	"?(format) {s = F''text{var}text''}"
	"?(str append) {s + a}"
	"?(str prepend) {s - a}"
	"?(str compare) {if(s = a)}"
	"?(str compare_not) {if(s = a = 0)}"
	"?(str icompare) {if(s ~ a)}"
	"?(str icompare_not) {if(s ~ a = 0)}"

category math :
	">\System\Functions\Math" ">\User\Functions\Math"
	iif
	pow fmod sqrt modf exp log log10 cos sin tan acos asin atan ceil floor abs fabs _hypot
	Crc32 Round RandomNumber RandomInt ConvertSignedUnsigned
	"?str RandomString encrypt decrypt"
	_operator

category multimedia :
	">\System\Functions\Multimedia" ">\User\Functions\Multimedia"
	bee

category qm :;;    QM items, threads, misc info, exe functions, etc
	">\System\Functions\Qm" ">\User\Functions\Qm"
	mac dis opt getopt qmitem newitem atend shutdown net lock out
	_hwndqm _unicode _portable _command _qmver_str _qmdir _qmfile QMVER EXE __FUNCTION__
	"?str getmacro setmacro"
	QmKeyCodeToVK QmKeyCodeFromVK SilentImport SilentExport IsThreadRunning EnumQmThreads GetQmThreadInfo
	GetToolbarOwner GetLastSelectedMenuItem IsLoggedOn EnsureLoggedOn PidlToStr PidlFromStr
	QmRegisterDropTarget QmUnregisterDropTarget GetExeResHandle ExeExtractFile ExeGetResourceData
	GetQmCodeEditor InsertStatement RecGetWindowName CompileAllItems Statement GetCallStack UnloadDll RtOptions
	PerfFirst PerfNext PerfOut IsValidCallback RedirectQmOutput GetQmItemsInFolder
	SpecFoldersMenu RegExpMenu OnScreenRect OnScreenDisplay OsdHide ErrMsg

category sys :;;    various system functions: registry, shutdown, tray icon, processes, environment variables, etc
	">\System\Functions\Sys" ">\User\Functions\Sys"
	rset rget shutdown
	GetCPU GetDiskUsage SetPrivilege RegisterComComponent IsLoggedOn EnsureLoggedOn
	EnumProcessesEx ProcessNameToId GetProcessExename GetProcessUacInfo GetProcessIntegrityLevel
	Tray Dde Wsh Services

category sysinfo :;;    get/set system settings and info: version, display, user, etc
	">\System\Functions\Sysinfo" ">\User\Functions\Sysinfo"
	GetUserInfo IsUserAdmin SystemParametersInfo GetSystemMetrics
	MonitorFromIndex MonitorFromWindow MonitorFromPoint MonitorFromRect
	_winnt _winver _iever _win64 _monitor

category script :;;    other languages: VBScript, C, C#, etc
	">\System\Functions\Scripting" ">\User\Functions\Scripting"
	__Tcc MSScript

category flow :;;    flow control: if, repeat, etc; compiler directives
	goto if else ifa ifk sel case rep for foreach break continue err ret atend call iif end
	SelInt SelStr

category _debug :;;    debug, errors, optimize, log. Added in QM 2.3.2.
	">\System\Functions\ErrDebug" ">\User\Functions\ErrDebug"
	out mes deb err perf opt getopt bee atend end
	OnScreenDisplay OsdHide OnScreenRect ErrMsg LogFile outb outw ClearOutput ExeOutputWindow
	GetTickCount timeGetTime PerfFirst PerfNext PerfOut GetLastError OutWinMsg CompileAllItems Statement GetCallStack UnloadDll IsValidCallback RedirectQmOutput
	_error _hresult _logfile _logfilesize __FUNCTION__
	"?str dllerror"

category _operator :;;   operators. Added in QM 2.3.2.
	"?{arithmetic add,    r = a + b} +"
	"?{arithmetic sub,    r = a - b} -"
	"?{arithmetic mul,    r = a * b} *"
	"?{arithmetic div,    r = a / b} /"
	"?{arithmetic modulo,    r = a % b} %"
	"?{bitwise AND,    r = a & b} &"
	"?{bitwise OR,    r = a | b} |"
	"?{bitwise AND NOT,    r = a ~ b} ~"
	"?{bitwise XOR,    r = a ^ b} ^"
	"?{bitwise right shift,    r = a >> b} >>"
	"?{bitwise left shift,    r = a << b} <<"
	"?{compare equal,    if(a = b)} ="
	"?{compare not equal,    if(a ! b)} !"
	"?{compare,    if(a < b)} <"
	"?{compare,    if(a <= b)} <="
	"?{compare,    if(a > b)} >"
	"?{compare,    if(a >= b)} >="
	"?{logical AND,    if(a and b)} and"
	"?{logical OR,    if(a or b)} or"
	"?{negative,    r = -a} -"
	"?{logical NOT,    if(!a)} !"
	"?{bitwise NOT,    r = ~a} ~"
	"?{address of,    r = &varOrFunc} &"
	"?{reference set,    &refVar = var} &"
	"?{indirect get,    r = *ptr} *"
	"?{indirect set,    *ptr = a} *"
	"?{cast type,    r = +a} +"
	"?{string to utf16,    r = @string} @"
	"?{string format,    r = F''text{var}text''} F"
	"?{array element,    r = a[i]} ["
	"?{string character,    r = s[i]} ["
	"?{member,    r = var.memberVar} ."
	"?{member,    r = var.Func(a b)} ."
	"?{conditional,    r = iif(condition a b)} iif"
	"?{function,    r = Function(a b)} ("
	"?{priority,    r = a + (b * c)} ("
	"?{str append,    s + a} +"
	"?{str prepend,    s - a} -"
	"?{str compare,    if(s = a)} ="
	"?{str compare_not,    if(s = a = 0)} ="
	"?{str icompare,    if(s ~ a)} ~"
	"?{str icompare_not,    if(s ~ a = 0)} ~"
	math

category _other :;;    Several subcategories. All added in QM 2.3.2.
	__rt_option __declaration __directive __programming_misc __qm_dll __in_dlgproc __variable_examples __array_examples __not_this

category __rt_option :;;   run-time otions. Added in QM 2.3.6.
	opt getopt spe RtOptions

category __declaration :
	def function dll type class category interface typelib ref

category __directive :
	"?{ } #if #else #endif #ifdef #ifndef #compile #set #opt #err #out #warning #error #exe #ret #sub #region #endregion"
	QMVER EXE _winnt _winver _iever _win64 _unicode

category __programming_misc :;;    misc. QM functions and classes for programming
	">\System\Functions\Programming" ">\User\Functions\Programming"
	__MemBmp __Font __Stream __MapIntStr __Settings __RegisterHotKey __ProcessMemory __Tcc __ImageList
	__RegisterWindowClass __OnScreenRect __MinimizeDialog __Tooltip __Accelerators __ComActivator __SharedMemory __TempFile
	__GlobalMem __Handle __Hicon __GdiHandle __HFile __Hdc __Drag
	"qm.exe"

category __qm_dll :;;    all dll functions and COM interfaces exported by qm
	"qm.exe" IStringMap IXml IXmlNode ICsv _qmfile

category __in_dlgproc :;;    functions for use in dialog procedures
	">\System\Dialogs\Dialog Functions" ">\System\Dialogs\Dialog Control Classes"

category __variable_examples :
	"?{ } {int i} {str s}"
	"?{  } {double d} {OTHERTYPE v}"
	"?{ (global)} {int+ g_i}"
	"?{ (thread)} {int- t_i}"
	"?{ (thread, private)} {int-- t_i}"
	"?(reference) {int& r = var}"
	"?(set reference later) {int& r;  &r = var}"
	"?(pointer) {int* p = &var}"
	"?(declare multi) {int a b c}"
	"?(declare-assign) {int i = 10}"
	"?(declare-call) {str s.from(a b)}"
	"?(declare-assign multi) {int a(10) b(20) c(30)}"
	"?(environment) {SetEnvVar(''e'' s)} {str s;  GetEnvVar(''e'' s)}"
	"?(free explicitly) {strVar.all} {interfaceVar = 0}"
	"?(new COM object) {Typelib.Class var._create}"
	"?(new QM COM object) {IXml x = CreateXml}"

category __array_examples :
	"?{ } {ARRAY(int) a} {ARRAY(str) a}"
	"?(create) {ARRAY(int) a;  a.create(10)}"
	"?(create 2-dim) {ARRAY(int) a;  a.create(4 10)}"
	"?(from string) {ARRAY(str) a;  a = ''a[91]]b[91]]c''}"
	"?(to string) {str s;  s = a}"
	"?(elem) {a[i] = b} {b = a[i]} {TYPE& r = a[i];  r.a = b} {TYPE& r = a[i];  b = r.a}"
	"?(elem 2-dim) {a[i j] = b} {b = a[i j]}"
	"?(add elem) {a[91]] = b} {TYPE& r=a[91]];  r.a = b} {int newIndex = a.redim(-1)}"
	"?(free explicitly) {a = 0}"

category __not_this :;;    use __not_this.GlobalFunction in class member functions to access a global function etc if its name matches a member name

 obsolete
#opt hidedecl 1
type crt :
type api :
type inet :
type io :
#opt hidedecl 0
