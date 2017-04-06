/exe
 out
#compile "__Exc"
 atend sub.Atend3; atend sub.Atend2; atend sub.Atend1
out 1
sub.Test
out 2


#sub Test
ExcThree et
ExcThree et2=et
 ExcOne et
 ExcOne et2=et
 TODO: also test ARRAY, _new, ret, arguments. Test new(ARRAY), ret(ARRAY), arg(ARRAY)
 ExcOne* p.
ARRAY(ExcOne) a1 a2
a1.create(3)
 a2.create(1)
a2=a1

 ExcThree- t_et.o=8
 ExcThree+ g_et
 min 0
 ARRAY(ExcThree)- t_a.create(2)

 ____________________

 ExcThree* p
 ExcOne* p
 p._new
  p._new(2)
 for(_i 1 3) out "resize:"; p._resize(_i); out p._len
 ____________________

 VARIANT v=et ;;type mismatch (ok)
 ____________________

 ARRAY(ExcThree) a
 a.create(2)
 a.redim(2)
 a.redim(1)
 a=0
 rep(2) a[].o.one=1
 a.insert(0)
 a.remove(0)

 sub.Ret
 ExcOne k.one=88
 ExcOne m=k
 sub.Arg(k)
 k=sub.Ret

out "Test"

 p._delete


#sub Arg
function ExcOne'a
out a.one


#sub Ret
function'ExcOne
 ret
ExcOne x
out "Ret"
ret x


#sub Atend1
out "Atend1"
min 0
out "kkk"
#sub Atend2
out "Atend2"
end
out "kkkkkk"
#sub Atend3
out "Atend3"

 TODO: Also test assigning to non-UDF (garbage_r).
 TODO: test in exe too.

 BEGIN PROJECT
 main_function  Macro2654
 exe_file  $my qm$\Macro2654.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  4
 guid  {100FB333-F670-4895-B4EA-42346D43EAD9}
 END PROJECT
