assert, verify, trace, trace_stack. Add QM item option "Disable assert/verify/trace".

RunTextAsFunction2 etc: make intrinsic. Now unreliable, eg when multiple threads call it simultaneously, especially when with sub.

keyboard functions and features:
   PostKeys(key(...) hwnd)
   PostCharacters("" hwnd)

ARRAY
   .find.
   Rename .redim to .resize.
   .tostr(s [$separator])
   .fromstr(s [$separator]).
   ARRAY(int) a="1 2 3"
   fromstr/tostr/etc: support 2-dim.
   Maybe in QM3.

Finding controls and acc by path (something like XPath):
   acc("class,role,name,etc/class,role,name,etc/etc")
   or could support this in navig string
   or acc(...).acc(...).etc

UI Automation.

To run PowerShell code add function PsExec that uses C# to execute PowerShell code. Now PsCmd/PsFile use command line.

Need new email functions. Don't use MailBee. Don't use Outlook account settings.

lef etc: When window activated, if other window is at that point, minimize other window(s) until the tartget window is at that point. Eg to click desktop. Maybe use only for desktop.

IMapIntInt, IMapInstStr, IMapStrInt.

Class Win:  Find(), FromAcc(), Name(), ...

foreach: allow member functions.

VbsOptions(flags [timeout] [hwndowner]). flags: 1 use global var, 2 no UI.

str.setsel(0 invalidhandle): must be error

Function: lenutf8(string [nbytes])

New PCRE (with compiling to native code). In a dll. Use new functions or class. Leave the old PCRE as is (findrx/replacerx).

foreach: IStringMap, ICsv, IXml, ARRAY.

Spec thread var _locale. Use with OLE type conversion functions.

DATE: gettimepart, getdatepart

val: add 4-th parameter - flags. Or in 2-nd parameter.
	2-nd parameter 4 - support "a|b|c".

__Eval: make public.

outv: display variable, like s="value" or r={left=0, top=0...}.
   Without arguments - all local variables.
   Maybe available only in QM. Can take code from DebugMode.cpp.
   This is from Wish List, maybe several times.

wait:
   wait for window idle
   wait for file unlocked/locked
   wait KF: implement in exe using LL hook.
   wait for a focused control. For example, in memory stress, when window activated, there may be no focused control (focus window is 0 or parent), and then key may fail.
   waiting for multiple events simultaneously, eg for window 1 or window 2, or for key or mouse. Eg create CWait object, add events, then wait.
   wait D [process]: disk usage.
   wait P [process|hthread]: CPU usage of a process or thread.

add function that fills web form in IE. Add floating toolbar dialog to capture form data. For example using HtmlDoc.GetForm.

Services: instead of ActiveX, use own services class. It should be implemented in QM service. ActiveX on Vista fails and even does not throw errors.

Version class

str.expandpath:
   use PathCanonicalize to process ..\ and .\
   expand "$sf$\{clsid}"; unexpand "::{clsid}"

dir and some other functions should support ":: idlist". Only if certain flag is set.

inp: option to show only small edit field eg over taskbar. See inp9 function. Also maybe with combo box.

add function that retrieves last keyboard/mouse input info

run:
   Check if good desktop. If UAC desktop, wait. Or add function, eg WaitForGoodDesktop.

ShowDialog:
   automatically set icon/bitmap for Button controls with BS_ICON/BS_BITMAP style. Eg "[1]iconfile" or "text[1]iconfile". To display text too, remove the style before calling BM_SETIMAGE. Note: BM_SETIMAGE removes visual styles.

Intrinsic dialog class. Intrinsic ShowDialog.

Dialogs: DlgProc class, including CB_Fill and listview support.

newitem:
   QMITEM&qi
   newitem ... [password]: would allow to modify or replace encrypted macros (see elden.brooks post in forum). Currently can only create new encrypted macro using template.
   flag to not open the item (and no undo).

JSON

str.getclip, getsel: get list of files

Htm.Navigate; Htm.Index; htm flag to search only forward from specified index.

RegisterEndMacroHotkey

acc and htm: option to search in IE thread. With acc, when searching in same thread, finds 22 times faster. htm - 15 times.

cmp. Flags: insens, locale, natural (numbers), wildcards.

opt verbose.

opt waitprecise: while waiting for something, check condition more frequently.

opt retry n: http://www.quickmacros.com/forum/viewtopic.php?f=3&t=5272

Storage class.

WindowInfo class.

SysInfo class.

strings: find in list.

rep 5.0 ;;repeat for 5 seconds.

call "dll>function"

Binary str.findreplace.

LogEvent (now Wsh has)

shutdown -6 0 "Function" should not kill Function if it is current function. Or, only if force is 1. Rename force to flags.

Rename or make alias:
	mes -> MsgBox or MessageDialog. Now is another function MsgBoxAsync.
	inp -> InputDialog. Now is another function InputBox.
	inpp -> PasswordDialog
    OnScreenDisplay -> osd
	ShowText -> TextDialog
	OpenSaveDialog -> FileDialog
	BrowseForFolder -> FolderDialog
	

Maybe should rename some functions, or/and make UDF from intrinsic:
	end: throw and endthread
	shutdown: many UDF
	wait: many UDF
	mes, list, inp, inpp: UDF mbox, ibox, lbox, pbox
	max etc: UDF
	cop etc: UDF
	str.encrypt, decrypt, escape: many UDF
	Transparent: WinSetTransparent

qmitem: flag "skip System"

str.replacerx: if callback function is used, does not expand $ in replacement.

Mixer class. Many users need to control volume and show volume level.

lef etc: use POINT variable, like POINT p; ...; lef p (from forum wish list)

Add set of functions (maybe COM class) to manipulate QM. Info about items, threads, etc, separate functions to add, delete, etc.

ocr: find text on screen using OCR method. Try textract SDK ($210).

ExcelSheet.GetAll etc, gets TRUE as -1 and FALSE as 0. Using ARRAY(VARIANT) a=Range.Value gets correctly. See \Notes\code\VARIANT bool to str.

findrx and str.replacerx should support binary. findrx should support it when when ito is specified.

findb flag for case insensitive.

inputpipe: a macro thread would create one-way pipe, then hooks would write to it, then the thread would read from it.

OnScreenDisplay flag: close when user clicks somewhere.

run: if opt err 1, implicitly add flag 0x100 (no error message).

$ operator - expand path. Usage like of @ operator, so should be not very difficult to add. Or use % for expand, and $ let be "convert integer to string".

Http.Post etc: async, timeout.

str.encrypt(a b c d length). Use length to encrypt any binary data without storing to str variable.

paste/setsel: try to restore all clipboard formats. Try OleDuplicateData.

Sqlite class (System): add findrx, matchw. Add custom collation that supports case-insens Unicode fully.

__Tcc: add exception handling.
    Try: http://sourceforge.net/projects/tcc-exceptions/
    See also: http://members.gamedev.net/sicrane/articles/exception.html
    And: http://www.codeproject.com/Articles/82701/Win-Exceptions-OS-Level-Point-of-View

mou "binary->lzo->base64"

AddTrayIcon:
	Flag to add icon in QM thread. Because if the thread is blocked, the icon is unresponsive and does not work Ctrl+click, onlclick and onrclick.
	Support AddTrayIcon ... "sub.OnRClick"

AutoPassword: May be slow in web pages. Add flags: Reverse order. Use FF mode (tag="INPUT" type="password").

________________________________________________

 REJECTED

child: path, eg "2/7/3" (like frame with htm). Not very actual when we have matchindex.

blockinput:
   Implement using QM hooks. However could not be used in exe.
   Option to replay what was blocked.

scan: specify a nonrectangular search region.
   wishlist: http://www.quickmacros.com/forum/viewtopic.php?t=1148
   eg a mask bitmap or region.
   now it is possible in UDF: get screen pixels, add mask and pass the bitmap to scan.

scan: get accessible object from image (which can be in background window). Then could click it without mouse. Same for html element, and for control.
   Can implement it with UDF. Call the UDF after scan, and pass the RECT var that is filled by scan.

Splitter control: add to System.
