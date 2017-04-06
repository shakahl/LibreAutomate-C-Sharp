function! $map $icons [flags] ;;flags: 0 list of icons, 1 bmp, 2 himagelist

 Adds icons to all menus in current thread.
 Returns 1 if succeded, 0 if failed.
 Fails if map syntax invalid or fails to load the bmp file. If fails to load an icon file, succeeds but does not display the icon.

 map - string that maps menu item ids to icon indices. Format: "id=icon id=icon ...". Icon index is 0-based. Example: "5=0 10=1 3=2 4=1".
 icons - list of icons or imagelist. Depends on flags 1 and 2.
 flags:
   0 - icons is list of icon files, like "$qm$\mouse.ico[]shell32.dll,5".
   1 - icons is imagelist file created with QM imagelist editor, like "$qm$\il_qm.bmp".
   2 - icons is imagelist handle. For example you can pass __ImageList variable.
      The imagelist must contain small icons (16x16).
      The function does not copy and does not destroy it.
      The imagelist will be used every time a menu is shown in current thread, therefore must not be destroyed too early.

 REMARKS
 Call once in thread, at any time before creating/showing menus. Later can call again if need to change icons.
 Works with menu bar menus, popup menus (to set item ids, you need function ShowMenu or class MenuPopup), and with system menu.
 Does not show icons in menus that can be scrolled because contain many items.
 Tip: Use smaller icons for items that can be checked. The function draws custom checkboxes. On Windows 2000/XP, and sometimes on Vista/7, there is not enough space...


class __MenuIcons
	-m_hh -m_flags ;;1 destroy imagelist
	-m_il -ARRAY(POINT)m_map
	-m_pen

__MenuIcons- __t_mi
ret __t_mi.AddIcons(map icons flags)
