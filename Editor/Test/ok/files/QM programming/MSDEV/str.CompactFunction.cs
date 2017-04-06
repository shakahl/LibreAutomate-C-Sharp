function [flags] ;;flags: 1 (WINAPI* Func)

replacerx("//.*$" "" 8)
replacerx("\s{2,}" " ")
replacerx("\( +" "(")
replacerx(" +\)" ")")
replacerx(" +\| +" "|")
replacerx("\b(__in|__out|__inout|__in_opt|IN|OUT) ")
replacerx("\b(__in_ecount|__in_ecount_opt)\( *\w+ *\) ")
replacerx(" \* *" "* ")
if(flags&1) replacerx("\bWINAPI (\w+)\(" "(WINAPI* $1)(" 4)
trim
this+"[]"
