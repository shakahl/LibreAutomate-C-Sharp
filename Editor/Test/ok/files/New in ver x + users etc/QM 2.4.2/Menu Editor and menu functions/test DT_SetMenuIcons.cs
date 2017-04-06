 SetThreadMenuIcons "501=1 502=2" "$qm$\il_qm.bmp" 1
DT_SetMenuIcons "501=1 502=2 552=3" "$qm$\il_qm.bmp" 1

str md=
 BEGIN MENU
 &Open :501 0x0 0x3 Co
 &Save :502 0x0 0x8 Cs
 -
 >Submenu :7
 	Item1 :551 0x200 0x8
 	Item2[9]Ctrl+K :552 0x200
 	<
 END MENU

int i=ShowMenu(md); out i

 MenuPopup m.Create(md)
 MENUINFO mi.cbSize=sizeof(mi); mi.fMask=MIM_STYLE; mi.dwStyle=MNS_NOCHECK
 SetMenuInfo m &mi
 m.Show
