typelib Excel
Excel.Application ap._create
ap.Visible=-1
act "Excel"
str s.expandpath("$documents$\txt.txt")
ap.Workbooks.OpenText(s @ @ xlDelimited xlDoubleQuote 0 -1 0 0 0 0 @ VbsEval("Array(Array(1, 1), Array(2, 2), Array(3, 4))"))

 ARRAY(VARIANT) a.create(3)
 ARRAY(int) 
 a[0 0]=1
 a[1 0]=1
 a[0 1]=2
 a[1 1]=2
 a[0 2]=3
 a[1 2]=4
 ap.Workbooks.OpenText(s @ @ xlDelimited xlDoubleQuote 0 -1 0 0 0 0 @ a)


 ap.Workbooks.OpenText(s @ @ @ 1)
 ap.Workbooks.OpenText(s @ @ xlDelimited xlDoubleQuote 0 -1)

 out "" xlGeneralFormat xlTextFormat xlMDYFormat xlYMDFormat
