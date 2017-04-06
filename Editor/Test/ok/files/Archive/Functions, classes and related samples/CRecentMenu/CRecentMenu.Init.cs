function hDlg submenuId nItems $registryKey [flags]

 Call this function on WM_INITDIALOG.

 hDlg - handle of window that contains menu bar.
 submenuId - id of menu item that will be used for "Recent" submenu.
   Initially it must be normal item, not submenu.
   Items ids in the submenu will start from this value.
 nItems - max number of items in "Recent" submenu.
 registryKey - registry key that will be used to save recent file paths.
 flags - currently not used and must be 0.


m_hdlg=hDlg
m_first_id=submenuId
m_nitems=nItems
m_rkey=registryKey
m_flags=flags
m_hsubmenu=CreatePopupMenu

MENUITEMINFOW mi.cbSize=sizeof(mi)
mi.fMask=MIIM_SUBMENU
mi.hSubMenu=m_hsubmenu
SetMenuItemInfoW GetMenu(hDlg) submenuId 0 &mi
