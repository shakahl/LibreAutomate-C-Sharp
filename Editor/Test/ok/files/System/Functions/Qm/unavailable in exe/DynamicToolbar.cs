 \
function# $text [$tempitemname] [hwndOwner]

 Shows toolbar created at run time.
 Returns toolbar window handle.

 text - menu text. Same as in usual popup menus.
 tempitemname - name of QM item to create for the toolbar. Default: "temp_toolbar". QM 2.2.0: can be full path, eg "\folder\temp_tb".
 hwndOwner - owner window handle.


opt noerrorshere 1

str folder; int fl=1|128
if(empty(tempitemname)) tempitemname="temp_toolbar"
else if(tempitemname[0]='\') folder.getpath(tempitemname ""); tempitemname+folder.len+1; fl~128
str cmd; if(hwndOwner) cmd=hwndOwner

ret mac(newitem(tempitemname text "Toolbar" "" folder fl) cmd)
