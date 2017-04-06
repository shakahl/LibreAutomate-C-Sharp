 This macro will be displayed when you click this class name in the editor and press F1.
 Add class help here. Add code examples below EXAMPLES.

 EXAMPLES

#compile "__CGet"
 CGet x y
 x=y

 ARRAY(CGet) a.create(3) b.create(3)
 a=b
 a[1]=x
 y=a[1]

 type TWA1 CGet'a
 TWA1 x y
 x=y

type TWA ARRAY(CGet)'a
TWA x y
x=y

 CGet g
 g=Function72(g)
  Function72(g)

 ARRAY(CGet) g
 g=Function73(g)

 ARRAY(CGet)* g
 g=Function74(g)
