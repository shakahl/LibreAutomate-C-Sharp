str s
 get clipboard text
s.getclip ;;or s.getsel to copy selection

 as an example, convert it to uppercase and replace spaces to commas
s.ucase
s.findreplace(" " ",")

 Similarly you can perform other operations on the string that is stored
 in variable s. There are no dialogs to insert string manipulation functions.
 You will find the list of available functions in QM Help, Reference (just
 click the Help button on the toolbar).

 place it to the clipboard
s.setclip ;;or s.setsel to paste directly

 This works well with simple text, but if you are working with rich
 text, the formatting (colors, styles, etc) probably will be lost.
