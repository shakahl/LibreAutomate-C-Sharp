 /
function hmenu [nLeave]
//nLeave: if negative, delete from beginning and leave -nLeave at the end of the menu

int i=GetMenuItemCount(hmenu);
if(nLeave<0) i+=nLeave; nLeave=0;
for(i i-1 nLeave-1 -1) DeleteMenu(hmenu, i, MF_BYPOSITION);
