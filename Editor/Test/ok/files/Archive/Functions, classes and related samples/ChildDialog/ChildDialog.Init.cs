function ctrlid [$macro] [dlgproc] [!*controls] [param] [bcolor]

 Call this function before calling ShowDialog for the main dialog.

 ctrlid - id of a Static control in main dialog. The child dialog will inherit its position, dimensions and id, and will destroy it.
 macro, dlgproc, controls, param - what you would use with ShowDialog to create the child dialog.
 bcolor - background color. If 0, uses default dialog color.

 REMARKS
 For exe, you may have to explicitly add macro containing child dialog definition. Before calling this function, insert:
   #exe addtextof "macro" ;;here macro is macro containing child dialog definition, for example "dlg_cd_simple_child".


m_ctrlid=ctrlid
m_macro=macro
memcpy &m_dlgproc &dlgproc 4*4
if(bcolor) m_bcbrush=CreateSolidBrush(bcolor)
