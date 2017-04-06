\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Could not find a good and quite easy way to add icons to standard menus.
 Can use these ways:
 1. Owner-draw menu. Very much work. Used in QM user menus.
 2. SetThreadMenuIcons. Quite dirty, not completely reliable, something is not perfect.
 3. SetMenuItemBitmaps or SetMenuItemInfo/MIIM_CHECKMARKS. If not Vista theme, looks bad and must be smaller than 16x16 icon. Also need many HBITMAPs.
 4. SetMenuItemInfo/MIIM_BITMAP/HBITMAP. Similar to 3, but worse.
 5. SetMenuItemInfo/MIIM_BITMAP/HBMMENU_CALLBACK/WM_MEASUREITEM/WM_DRAWITEM. Would be good, but no theme.

if(!ShowDialog("dialog_menu_icons" &dialog_menu_icons 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 8 12 48 14 "Menu"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_MEASUREITEM
	out "m"
	MEASUREITEMSTRUCT& r=+lParam
	 out "%i %i" r.itemHeight r.itemWidth
	 r.itemHeight=32
	 r.itemWidth=32
	case WM_DRAWITEM
	out "d"
ret
 messages2
sel wParam
	case 3 goto gMenu
	case IDCANCEL
ret 1

 gMenu

SetThreadMenuIcons "1=2 2=11 3=12 7=3 8=4" "$qm$\il_qm.bmp" 1

str s=
 1 &Normal
 2 &Checked
 3 &Radio
 -
 4 &No icon
 5 &No icon, checked
 6 &No icon, radio
 -
 >7 Submenu
 	8 Normal2
 	<

MenuPopup m.AddItems(s)
m.CheckItems("2 5")
m.CheckRadioItem(3 3 3)
m.CheckRadioItem(6 6 6)


MENUITEMINFOW x.cbSize=sizeof(x)
x.fMask=MIIM_BITMAP
  x.hbmpItem=HBMMENU_MBAR_CLOSE
  x.hbmpItem=-1
 __GdiHandle b=LoadPictureFile("$qm$\bitmap1.bmp")
 x.hbmpItem=b
 out SetMenuItemInfoW(m 1 0 &x)
 x.hbmpItem=-1
 out SetMenuItemInfoW(m 3 0 &x)

 __GdiHandle b=LoadPictureFile("$qm$\bitmap1.bmp")
 out SetMenuItemBitmaps(m 1 0 b b)
 out SetMenuItemBitmaps(m 2 0 b b)

int i=m.Show(hDlg 0 1)
out i
