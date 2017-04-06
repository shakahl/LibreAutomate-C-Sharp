 /
function str&names [$exename] [flags] [*cbFunc] [cbParam] [ARRAY(int)&handles] ;;flags: 1 exclude hidden, 2 exclude without name, 4 exclude popup, 8 exclude non-popup, 16 exclude popup without caption.

 Gets list of window names.
 Use this function to easily get list of windows for display or to add to a combo box etc.
 To get array of handles for other purposes, use <help>win</help>.

 names - str variable that receives list of window names. Can be 0.
   For windows without name, gets +class.
 exename - name of program. If not used, all programs.
 cbFunc - address of window enumeration callback function.
   Its must begin with:
     function# hWnd cbParam str&wintext str&winclass
   It must return 1 to continue enumeration, or 0 to stop.
 cbParam - some value to pass to the callback function.
 handles - receives window handles.


if(cbFunc and IsBadCodePtr(cbFunc)) end ERR_BADARG

type ___GWL str*names flags *cbFunc cbParam ARRAY(int)*handles str'ts str'cs
___GWL g
if(&names)
	g.names=&names
	int sflags=names.flags ;;save names flags
	g.names.flags=3
g.flags=flags
memcpy &g.flags &flags 16
g.ts.flags=1; g.cs.flags=1

opt hidden !(flags&1)
win "" "" exename flags&12|0x8000 &sub.EnumProc &g

if(&names) names.flags=sflags ;;restore names flags

 note: this was public. Fbc don't change parameters etc.


#sub EnumProc
function# hWnd ___GWL&g

if(g.flags&16)
	int style=GetWindowLong(hWnd GWL_STYLE)
	if(style&WS_CAPTION != WS_CAPTION)
		if(style&WS_POPUP) ret 1

if(g.names or g.flags&2)
	err-
	g.ts.getwintext(hWnd)
	if(!g.ts.len)
		if(g.flags&2) ret 1
		if(g.names) g.cs.getwinclass(hWnd); g.ts.from("+" g.cs)
	err+ ret 1 ;;if the window destroyed
	if(g.names) g.names.addline(g.ts)

if(g.handles) ARRAY(int)& ha=g.handles; ha[]=hWnd

if(g.cbFunc) ret call(g.cbFunc hWnd g.cbParam &g.ts &g.cs)

ret 1
