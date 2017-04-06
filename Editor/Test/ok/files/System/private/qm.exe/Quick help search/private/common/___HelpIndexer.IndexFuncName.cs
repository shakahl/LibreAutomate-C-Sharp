 /
function IStringMap&m $funcName wordWeight

str s=funcName
s.findreplace("_" " ")
s.replacerx("\d+" " ")
s.replacerx("(?<![A-Z])[A-Z]" " $0")
s.formata(" %s" funcName) ;;add full name too
 out s
IndexText(m s wordWeight)
