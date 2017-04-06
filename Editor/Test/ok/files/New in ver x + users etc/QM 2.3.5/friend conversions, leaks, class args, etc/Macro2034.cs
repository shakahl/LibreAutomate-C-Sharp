BSTR b="test"
 b.alloc(10)
 out b[4]
 out b[5]
 out b

 BSTR bb
 out b.cmp("Test" 1)

out b.len
b.alloc(20)
out b
b.free
out b.len
out "%i" b.pstr
