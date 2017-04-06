 /
function $menuCSV [flags] ;;flags: 1 menuCSV is file, 2 menuCSV is macro, 4 add 'Edit menu'

 Shows menu. You can drag-drop menu items to text fields in any window to paste the text there.

 menuCSV - CSV containing menu items.
   Can be 1 or 2 or 3 columns.
   First column - menu item labels. Also you can create submenus and separators, like with <help>ShowMenu</help>.
   Second column - text to paste. If column or text is missing, pastes first column's text.
   Third column can be name of macro to run instead of pasting text. To get text in the macro: function $text
 flags:
   1, 2 - CSV is in file or macro. menuCSV is file path or macro name or \path.
   4 - add 'Edit menu' menu item that opens the file or macro in QM. This flag is used with flag 1 or 2.

 REMARKS
 To paste text, you can click or drag-drop a menu item.
 Use Ctrl to select existing text to replace. In multiline text it replaces single line.
 After you drop, the menu remains, and you can drag-drop more items or click somewhere to end menu.
 If the drop target window is inactive, on drop it is activated. It ends menu too.
 You can right-click a menu item to copy its text to the clipboard.
 You'll see drag_drop_text_menu_manager thread in the Threads list, unless function drag_drop_text_menu_manager is in a private folder.


opt noerrorshere 1

int+ g_hwndDragDropTextMenuManager
if !IsWindow(g_hwndDragDropTextMenuManager)
	g_hwndDragDropTextMenuManager=0
	mac "drag_drop_text_menu_manager"
	wait 5 V g_hwndDragDropTextMenuManager; err ret
	0.05

lpstr s=q_strdup(menuCSV)
PostMessage g_hwndDragDropTextMenuManager WM_APP flags s
