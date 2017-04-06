 \
function iid flags x y cx cy

RECT- t_r; t_r.left=x; t_r.top=y
int- t_flags=flags
int- t_iid=iid
str name.getmacro(iid 1); err

MainWindow name "@QM_MacroProgress" &MPW_WndProc 0 0 cx cy WS_POPUP|WS_CAPTION 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST
