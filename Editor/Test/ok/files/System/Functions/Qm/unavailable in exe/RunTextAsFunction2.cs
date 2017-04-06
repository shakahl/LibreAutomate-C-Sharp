 \
function# $text [param]

 Executes text as function. Calls it synchronously, in current thread.
 The return value is the return value of the function. The function must be declared as returning int (#) or nothing.
 The function must have 1 parameter (even if it isn't used), which receives the raw value of param. The parameter must be of int or a pointer/reference type.

 EXAMPLES
 out RunTextAsFunction2("function# i[]inp i[]ret i[]" 7)

 type RTAF2 arg1 str'arg2
 RTAF2 r; r.arg1=3; r.arg2="ducks"
 str code=
  function RTAF2&r
  str s.format("%i %s" r.arg1 r.arg2)
  r.arg2=s
 RunTextAsFunction2(code &r)
 out r.arg2


opt noerrorshere 1

ret call(newitem(F"temp_function2_{getopt(itemid 3)}" text "Function" "" "" 1|128) param)
