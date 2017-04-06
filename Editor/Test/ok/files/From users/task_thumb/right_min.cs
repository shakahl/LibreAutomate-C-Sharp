int hwnd=win(mouse)
int icon=GetWindowIcon(hwnd)
hid hwnd
str code tbname title
tbname.from("THUMB_" hwnd)
title.getwintext(hwnd);title="Show"
code.format(" /mov %i %i /siz 38 38 /isiz 32 32 /hook _thumb_min_hook /ini ''$my qm$\Toolbars\task_thumb.ini''" xm ym)
code.formata("[]%s :_thumb_min(%i ''%s'') *%i" title hwnd tbname icon)
DynamicToolbar22(code tbname 0 "\task_thumb\tmp\")
DestroyIcon(icon)
