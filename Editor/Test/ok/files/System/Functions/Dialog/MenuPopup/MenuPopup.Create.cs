function $md

 Creates popup menu from menu definition.

 md - menu definition.
   If md is single line, it is interpreted as name of QM item (macro etc) that contains menu definition.

 REMARKS
 You can create menu definitions with the Menu Editor. Look in floating toolbar -> More Tools.
 Read more in <help #IDH_DIALOG_EDITOR#A13>Help</help>.
 This function does not activate hotkeys specified in menu definition.

 See also: <ShowMenu>, <MenuPopup.AddItems>, <DT_SetMenuIcons>.
 Added in: QM 2.4.2.

 EXAMPLE
 str md=
  BEGIN MENU
  &Open :1
  &Save :2
  -
  >Submenu
  	Item1 :11
  	Item2 :12
  	<
  END MENU
 
 int j=ShowMenu(md); out j


opt nowarningshere 1
if(m_h) DestroyMenu m_h; m_h=0
m_h=DT_CreateMenu(md 0 1)
