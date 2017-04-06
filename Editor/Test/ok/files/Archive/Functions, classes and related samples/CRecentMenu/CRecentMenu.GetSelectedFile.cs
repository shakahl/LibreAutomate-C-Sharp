function! menuItemId str&_file

 Call this function to get selected file path.
 Returns 1 if successful, or 0 if menuItemId is invalid.

 menuItemId - selected menu item id.
 _file - receives file path.


ret MenuGetString(m_hsubmenu menuItemId &_file)
