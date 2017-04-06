/s/i  ;;set "select text" and "case insensitive" options
 when typed "ename", replaces it with "John Smith":
name :"John Smith"
 when typed "eg,," or "Eg,,", replaces it with "for example, " or "For example, " (here /c is "capitalize first character" option):
/cg,, :key "for example, "
 when typed "eee", shows popup menu:
ee :mac "Sample menu2"

 The trigger includes Shift, but it is optional (grayed
 button in Properties), and therefore the first character
 you have to type can be either e or E.
