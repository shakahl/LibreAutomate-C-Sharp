 \
function $text

 Executes text as macro.
 Warning: Don't call this function several times in sequence.

 EXAMPLE
 RunTextAsMacro "mes 1[]mes 2[]"


opt noerrorshere 1

mac newitem("temp_macro" text "" "" "" 1|128)
