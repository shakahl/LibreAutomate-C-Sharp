 /
function# $caption $text [hwndOwner] [flags] ;;flags: 1 modeless, 2 QM-format, 4 text is file.

 Text box (dialog).
 Returns 0 if modal, window handle if modeless, -1 on error.

 caption - dialog title bar text. Default: "QM Textbox".
 text - text.
 hwndOwner - handle of owner window or 0.
 flags:
   1 - modeless dialog. Does not wait until closed.
     This thread must process messages. If hangs, insert 'opt waitmsg 1' before.
   2 - text is formatted as QM code.
     To display text of a QM item, pass item id in the high-order word of flags: flags=MakeInt(2 qmitem("name")). Then text is not used and can be "".
     Unavailable in exe.
   4 - text is file path. Displays file text.
     If rtf file, displays in rich text format.

 EXAMPLES
  show macro text:
 str s.getmacro("Macro1")
 ShowText "" s

  show text from file:
 ShowText "ReadMe" "$desktop$\readme.txt" 0 4

  show rich text from file:
 ShowText "Document" "$desktop$\document.rtf" 0 4


str controls = "0 3"
str Dlg Edit3

Dlg=iif(empty(caption) "QM Textbox" caption)
if(flags&4) Edit3.from("&" text); else Edit3=text

ret ShowDialog("ST_DlgProc" &ST_DlgProc &controls hwndOwner flags&1 0 0 flags)