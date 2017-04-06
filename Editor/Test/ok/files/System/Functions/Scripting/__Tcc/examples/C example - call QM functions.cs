 This macro shows how to add QM user-defined functions to C code.

int q1(&sub.Func1) q2(&sub.Func2)
__Tcc x3.Compile("" "main" 0 "" "QmFunc1[]QmFunc2" &q1)
out call(x3.f)


#sub Func1
function[c]# a b
int r=a+b
mes F"Func1 result={r}" "QM function" "i"
ret r


#sub Func2
function[c]# a b
int r=a+b
mes F"Func2 result={r}" "QM function" "i"
ret r


#ret

int main()
{
return QmFunc1(1,2) + QmFunc2(3,4);
}
