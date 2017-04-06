out
str s

 s="WorkinG"
  s="Worker"
 s="found"
 out s.stem()

 s="one WorkinG two"
  s[11]=0
 out s.stem(0 4 7)

 s="WorkinGs"
 out s.stem(0 0 7)

 s="WorkinG"
 out s.stem(0 0 50)

s="-Lovely, Worked, Working, happiness, happy, unhappy. works"
out s.stem(3)

out s.len
