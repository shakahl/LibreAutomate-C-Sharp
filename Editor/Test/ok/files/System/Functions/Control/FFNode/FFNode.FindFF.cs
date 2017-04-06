function! `hwnd $tag [$textOrHTML] [$attributes] [flags] [^waits] [matchIndex] [*cbFunc] [cbParam] ;;flags: 1 text *, 2 text rx, 4 att *, 8 attr rx, 64 direct child, 128 reverse, 0x1000 error if not found, 0x2000 search in UI, 0x10000 document always busy

 Finds a HTML node COM object in web page, and initializes this variable.

 REMARKS
 Usually you don't call this function directly, but use <help>Acc.FindFF</help>. It calls this function and converts the found node object to accessible object, which is more useful in most cases. Everything is the same.


node=0
 ______________________________

 CONTAINER

int hwndFF
FFNode fnDoc

sel hwnd.vt
	case VT_I4 ;;hwnd
	hwndFF=hwnd.lVal
	if(!IsWindow(hwndFF)) end ERR_HWND
	
	case VT_BSTR ;;window name etc
	hwndFF=sub_sys.WinFromStr(hwnd.bstrVal)
	if(!hwndFF) end ERR_WINDOW
	
	case [VT_UNKNOWN,VT_DISPATCH]
	err-
	if(hwnd.vt=VT_UNKNOWN) fnDoc=hwnd.pdispVal ;;FFNode
	else Acc _a.a=+hwnd.pdispVal; fnDoc.FromAcc(_a) ;;IAccessible
	err+
	if(!fnDoc) end ERR_BADARG
	
	case else end ERR_BADARG

 ______________________________

 PROPERTIES

type ___FFNODEFIND
	BSTR'tag ~textOrHTML ~attributes flags matchIndex *cbFunc cbParam
	cf1 cf2 nodeType
	ARRAY(str)a ARRAY(BSTR)a1 ARRAY(BSTR)a2 ARRAY(word)a3
	BSTR'bFRAME BSTR'bIFRAME

___FFNODEFIND e
e.nodeType=val(tag 0 _i); tag+_i; tag+strspn(tag " ")
e.tag=tag
e.textOrHTML=textOrHTML
e.attributes=attributes
e.flags=flags
sel(flags&3) case 1 e.cf1=3; case 2 e.cf1=33; case else e.cf1=17
sel(flags&12) case 4 e.cf2=3; case 8 e.cf2=33; case else e.cf2=17
e.cbFunc=cbFunc
e.cbParam=cbParam

if !empty(attributes)
	ICsv c=CreateCsv; c.Separator="="; c.FromString(attributes)
	int i n=c.RowCount
	e.a1.create(n); e.a2.create(n); e.a3.create(n); e.a.create(n)
	for(i 0 n) e.a1[i]=c.Cell(i 0); e.a[i]=c.Cell(i 1)

e.bFRAME="FRAME"; e.bIFRAME="IFRAME"
if(matchIndex<1) matchIndex=1

err+ end ERR_BADARG
 ______________________________

 FIND, WAIT

 gFind
e.matchIndex=matchIndex

if hwndFF
	fnDoc.FromDoc(hwndFF flags&0x12000)
	err if(_error.code=ERRC_OBJECT or (_error.code=ERRC_OBJECTGET and find(_error.description "Firefox")<0)) goto gNotfound; else end _error

int found=__Find(fnDoc e 0)
if found
	ret 1

 gNotfound
if waits>0
	opt waitmsg -1
	wait 0.5
	waits-0.5
	goto gFind
 ______________________________

if(flags&0x1000) end ERR_OBJECT
err+ end _error
