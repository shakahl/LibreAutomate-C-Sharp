function! `hwnd $role [$name] [$prop] [flags] [^waits] [matchIndex] [$navig] [*cbFunc] [cbParam] ;;flags: 1 use *, 2 regexp, 4 use * in value/descr, 8 regexp value/descr, 16 +invisible, 32 +useless, 64 direct child, 128 reverse, 0x1000 error, 0x2000 web page, 0x10000 document always busy

 Finds accessible object, and initializes this variable.
 Returns: 1 found, 0 not found. If flag 0x1000, error if not found.

 hwnd - where to search.
   Can be one of:
     Window handle, name or +class. If "" - active window. Can be top-level or child window. Must not be child if flag 0x2000.
     IAccessible variable (accessible object). If you have Acc a, pass a.a.
     FFNode variable.
 role - role.
 name - name.
 prop - list of other properties in CSV format.
   Format: "class=...[]id=...[]...". Any number, in any order.
   class - class name of direct container control or window. Must be full or with wildcard characters (regardless of flags).
   id - id of direct container window.
   value - value.
   descr - description.
   xy - top-left corner coordinates in client area.
   state - state and state mask.
   a:attribute - HTML attributes. Example: "a:id=x[]a:class=c".
   level - level of the object in the object hierarchy, relative to the direct container (class/id, or hwnd). The function does not evaluate objects at other levels, therefore can be faster. Direct children are at level 0. Can be two values - minimal and maximal level. Default: "0 0x7fffffff".
   wfName (QM 2.3.4) - Windows Forms (.NET) name of direct container control. Must be full or with wildcard characters (regardless of flags).
 flags - <help>acc</help> flags.
   Uses only flags specified above.
   Applies flags 4 and 8 only to the value and description strings in prop.
 waits - number of seconds to wait for the object.
 matchIndex - 1-based index of matched object.
 navig - post-navigation string.
 cbFunc - address of <help "Callback_Acc_Find">callback function</help>.
   A template is available in menu -> File -> New -> Templates.
   Read more about <help #IDP_ENUMWIN>callback functions</help>.
 cbParam - some value to pass to the callback function.

 REMARKS
 This function calls <help>acc</help>. Almost everything is the same, therefore not completely documented here.
 All strings are case insenitive. If used regular expression, case sensitive, unless it begins with (?i).
 To capture accessible objects and create code, use the "Find Accessible Object" dialog.

 Added in: QM 2.3.3.

 ERRORS
 ERR_OBJECT - not found (if flag 0x1000).
 ERR_HWND, ERR_WINDOW - hwnd invalid, or window not found.
 ERR_BADARG - prop CSV invalid; prop contains unknown properties; navig invalid; hwnd invalid when used IAccessible or FFNode.
 ERR_RX_PATTERN - invalid regular expression.
 ERR_FUNCADDRTYPE - cbFunc invalid.


flags&0xF32FF
flags|0x4100
if(cbFunc) flags|0x8000

sel hwnd.vt
	case [VT_DISPATCH,VT_UNKNOWN] ;;IAccessible, FFNode
	IAccessible _a=+hwnd.pdispVal; err
	if(!_a) end ERR_BADARG
	this=acc(name role _a "" prop flags cbFunc cbParam navig waits matchIndex)
	
	case else ;;hwnd, window name etc
	this=acc(name role hwnd "" prop flags cbFunc cbParam navig waits matchIndex)

ret this.a!0

err+ end _error
