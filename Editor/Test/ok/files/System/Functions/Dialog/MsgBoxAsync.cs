 \
function ~text [$caption] [$style] [flags] ;;flags: 1 sync

 Shows message box and does not wait.
 The caller macro does not wait until the message box is closed.

 text, caption, style - same as with <help>mes</help>.

 REMARKS
 Note: In exe the message box disappears as soon as exe process ends.

 Added in: QM 2.3.0.

 EXAMPLE
 MsgBoxAsync _s.format("%s %i" "test" 5) "test" "i"
 out 1


if(flags&1)
	mes text caption style
else
	mac "MsgBoxAsync" "" text caption style 1
