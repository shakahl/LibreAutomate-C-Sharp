 /
function# $lnkfile [flags] [timeout] ;;flags: 1 do not display a dialog

 Updates shortcut's target if it is moved or renamed.
 It is not error to call this function for healthy shortcuts too.
 Returns 1 if successful.

 lnkfile - shortcut file.
 timeout - if flag 1 used, the max number of milliseconds to search. Default (if omitted or 0): 3000.

 EXAMPLE
 str lnk="$desktop$\test.lnk"
 if(!UpdateShortcut(lnk)) out "invalid shortcut: %s" lnk


IShellLinkW sl._create(CLSID_ShellLink)
IPersistFile pf=+sl
str s.expandpath(lnkfile)
pf.Load(@s 0); err ret
int rflags=SLR_UPDATE; if(flags&1) rflags|SLR_NO_UI; rflags|timeout<<16
sl.Resolve(0 rflags); err ret
ret !_hresult
