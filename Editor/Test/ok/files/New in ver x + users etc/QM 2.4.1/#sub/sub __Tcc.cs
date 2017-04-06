 /exe
sub.Test

#sub Test
__Tcc x.Compile("" "func2") ;;when first argument is "", gets C code from this macro, below the #ret line
out call(x.f 4)

 BEGIN PROJECT
 main_function  sub __Tcc
 exe_file  $my qm$\sub __Tcc.qmm
 flags  6
 guid  {1E3F3887-50FA-4DEC-A696-AFF7A227827E}
 END PROJECT

#ret ;;QM does not compile macro below #ret as QM code

int func1(int x)
{
return x*x;
}

int func2(int x)
{
return func1(x);
}
