function! hm

dll- uxtheme
	#OpenThemeData hwnd @*pszClassList
	#CloseThemeData hTheme

if(_winnt<6) ret

 is classic theme or old common controls?
int- mt
if(!mt) mt=1; int h=OpenThemeData(0 L"menu"); if(h) CloseThemeData h; mt=2
if(mt!2) ret

 is multicolumn?
int i
for i 0 GetMenuItemCount(hm)
	if(GetMenuState(hm i MF_BYPOSITION)&(MF_MENUBARBREAK|MF_MENUBREAK)) ret

ret 1
