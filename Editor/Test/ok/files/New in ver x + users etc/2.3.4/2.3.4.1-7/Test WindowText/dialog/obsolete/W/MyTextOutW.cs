 /dlg_apihook
function# hdc x y @*lpString c

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnTextOutW hdc x y lpString c)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(!R) ret R

RECT r
r.left=x; r.top=y; r.right=x; r.bottom=y

CommonTextFunc 3 hdc lpString c &r

ret R
