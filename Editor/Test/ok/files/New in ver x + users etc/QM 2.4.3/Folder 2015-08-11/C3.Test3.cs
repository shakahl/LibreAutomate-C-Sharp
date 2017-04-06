C1 x
C2 y

out x.a ;;VC error even here, although out a not error
out x.b
 out y.a ;;error, because C2:C1 is protected and C3 is not inherited from C2. Before QM 2.4.3 no error, because a belongs to C1 and C3 is inherited from C1.
 out y.b ;;now error too
out y.c
out y.d

 out a
 out b
 out _.a

 y.Test1 ;;now error too
y.Test2
