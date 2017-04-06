 \
function# $text [~a1] [~a2] [~a3] [~a4]

 Executes text as function. It runs asynchronously, in separate thread.
 The function can have up to 4 parameters.
 Returns thread handle.

 REMARKS
 Caller macro does not wait until the function ends. To wait, use wait 0 H (see example), or use RunTextAsFunction2.

 EXAMPLES
 RunTextAsFunction "mes 1[]"

 int i
 wait 0 H RunTextAsFunction("function &i[]inp i[]" &i)
 out i


opt noerrorshere 1

lock
int+ ___rtaf_time
if(GetTickCount<___rtaf_time+100) 0.1
___rtaf_time=GetTickCount
lock-

ret mac(newitem(F"temp_function_{getopt(itemid 3)}" text "Function" "" "" 1|128) "" a1 a2 a3 a4)
