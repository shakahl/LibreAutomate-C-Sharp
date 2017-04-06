type A
type B :A r
type C :A'_base r
 type D :A[4] r
 type E :A'_[4] r

type POINTEX :POINT ex
 out "%i %i %i" sizeof(A) sizeof(B) sizeof(C)

A a
B b
C c
POINTEX x

out c.f
