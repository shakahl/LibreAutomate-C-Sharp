out
dll "qm.exe" #__Eval_old $s [*isOk]
str s

 s="100"
 s="-100"
 s="0 or 1"
 s="0 || 1"
 s="1 and 2"
 s="1 && 2"

 s="10+ -15"
 s="1+2*3"
 s="3>2*3"
 s="1+-(2*-3)"

  these not supported
 s="{ret 100}+{ret 12}"
 s="Dialog110" ;;cannot end thread, QM crashes
 s="Function215"
 s="Function215(5)"

out __Eval(s)
out __Eval_old(s)
