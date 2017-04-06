 /dialog_QM_Tools

str s sw
if(!_WinGetText(s sw 1)) ret
if(sw.len) sw+"[]"
s=F"{sw}_i={s}"
if(s.end("[]_i=_i")) s.fix(s.len-7)

sub_to.Test m_hparent s 0 1

#ret
function hDlg ~statement
#ret

if(!_i) sub_sys.MsgBox hDlg statement "Not found" "!"; end

err-
__OnScreenRect osr
int i; RECT r; DpiGetWindowRect _i &r
int w=GetAncestor(_i 2)
act w; 0.25
for(i 0 6) osr.Show(i&1*3 r); 0.25
act hDlg
err+
