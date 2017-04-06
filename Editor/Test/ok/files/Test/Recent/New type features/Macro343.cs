 type T1 a b
 type T2 :T1 x y
 T2 a
 a.b=7
 out a.T1.b

 type T3 :int x y
 T3 t
 t.int=8
 out t.int

 type T4 :str x y
 T4 t
 t.str="abc"
 out t.str

 type T5 __
 T5 t.__=4
 out t.__
