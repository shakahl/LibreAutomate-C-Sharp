 /
function% hWnd message wParam lParam

 Adds toolbar icons from an imagelist file created with QM imagelist editor.
 Call this function from toolbar hook function (before sel message).

 Toolbar syntax:
  /hook hook_function
  /imagelist "file.bmp"
 Label1 :command *:imageindex
 Label2 :command *:imageindex
 ...
 Also can contain buttons with icons not from the imagelist.


if(message!=WM_NOTIFY) ret
NMHDR* nh=+lParam
if(nh.code=TBN_GETDISPINFOW and nh.idFrom=9999); else ret

NMTBDISPINFOW* d=+nh
str s1 s2 s3
s1.getwintext(hWnd)
s2.getmacro(s1); err ret
s1.getl(s2 d.idCommand)
if(findrx(s1 "\*:(\d+)$" 0 0 s3 1)<0) ret
 out d.idCommand
 The above code takes ~20 mcs. It runs for some normal buttons too.
 The below code runs only at startup and after text or style changes.

int htb=id(9999 hWnd)
int il=SendMessage(htb TB_GETIMAGELIST 0 0)

__ImageList il2
int i(-1) j
foreach s1 s2
	i+1
	sel i
		case 0 continue
		case 1
		if(findrx(s1 "/imagelist +''(.+?)''" 0 0 s3 1)>0) il2.Load(s3)
		continue
		
	if(findrx(s1 "\*:(\d+)$" 0 0 s3 1)<0) continue
	int hi=ImageList_GetIcon(il2 val(s3) 0)
	if hi
		j=ImageList_ReplaceIcon(il -1 hi); if(j<0) j=I_IMAGENONE
		DestroyIcon hi
	else
		j=I_IMAGENONE
	
	SendMessage(htb TB_CHANGEBITMAP i j) ;;no more TBN_GETDISPINFOW
	if(i=d.idCommand) d.iImage=j


 Easier would be to add or replace icons in imagelist on WM_INITDIALOG.
 But then we cannot restore icons when toolbar changed (text or some properties).

 For some buttons, QM extracts icons when creating toolbar.
 For others extracts on demand (sets image index = I_IMAGECALLBACK and processes TBN_GETDISPINFOW).
 If explicitly specified icon begins with :, also uses I_IMAGECALLBACK.
