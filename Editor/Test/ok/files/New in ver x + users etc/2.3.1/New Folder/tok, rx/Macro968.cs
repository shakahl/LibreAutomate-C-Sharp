out
  This is example 2 from the tok help page:
  It produces the output as documented
 str s = "one, (two + three) four five"
 ARRAY(str) arr arr2
 int i nt
 nt = tok(s arr 3 ", ()" 8 arr2)
 for(i 0 nt) out "'%s' '%s'" arr[i] arr2[i]


  When I add a double quote inside the brackets, the behaviour is not what I expected
 str s = "one, (two'' + three) four five"
 ARRAY(str) arr arr2
 int i nt
 nt = tok(s arr 3 ", ()" 8 arr2)
 for(i 0 nt) out "'%s' '%s'" arr[i] arr2[i]

str s = "one, (two'' + three) four five"
ARRAY(str) arr arr2
int i nt
nt = tok(s arr 3 ", ()" 8)
for(i 0 nt) out "'%s'" arr[i]