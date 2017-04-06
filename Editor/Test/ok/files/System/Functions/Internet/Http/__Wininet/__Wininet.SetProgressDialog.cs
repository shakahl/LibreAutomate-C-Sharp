function dlg [$title]

 Sets progress dialog.
 Functions that support it:
   Ftp: FileGet, FileGetStr, FilePut, FilePutStr.
   Http: Get, Post, PostFormData. PostFormData shows dialog only when receiving response, not when uploading files.
   IntGetFile uses it internally.

 dlg:
   0 (default) - don't show progress dialog.
   1 - show progress dialog as unowned window.
   other - show progress dialog as owned window. dlg is owner window handle.
 title - dialog title bar text. If empty, shows text that depends on function.

 Added in: QM 2.3.2.


m_dlg=dlg
m_dlgTitle=title
