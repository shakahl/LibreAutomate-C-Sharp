int w=win("Document - WordPad" "WordPadClass")
int c=id(59648 w) ;;editable text 'Rich Text Window'
 out SendMessage(c WM_GETTEXTLENGTH 0 0)

str s.all(100000)
out SendMessage(c WM_GETTEXT s.nc s)
