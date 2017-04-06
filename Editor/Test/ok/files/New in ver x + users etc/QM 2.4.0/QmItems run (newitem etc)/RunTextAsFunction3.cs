 /Macro2180
 \
function# $text [param]

 Executes text as function, which is called synchronously, in current thread.
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

dll "qm.exe" #TestNewitem [$name] [$text] [$templ] [$trigger] [$folder] [flags] [debugFind]

long t1=perf
TestNewitem("temp_function" text "Function" "" "" 1|128)
 TestNewitem("f_function" text "Function" "" "" 1)
long t2=perf
_i=call(newitem("temp_function" text "temp_function_template" "" "\User\Temp" 1|128) param)
out F"{t2-t1} {perf-t2}"
ret _i
