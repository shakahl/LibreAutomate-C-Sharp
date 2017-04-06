BSTR b="Abc"
out b.add(" de")
out b.add("one " "two")
out b.cmp("one two")
b.alloc(10)
out b.len
b.free
b="Abc"
out b.add(b)
