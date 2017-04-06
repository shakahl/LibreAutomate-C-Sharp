out
str s="one two, threee two four"

 out s.findreplace("two" "R")
 out s.findreplace("tw" "R")
 out s.findreplace("tw" "R" 2)
 out s.findreplace("Two" "R" 1)
 out s.findreplace("two" "R" 0 " ")
 out s.findreplace("two" "R" 4)
 out s.findreplace("ee" "e")
 out s.findreplace("ee" "e" 8)
 out s.findreplace("two" "R" 2 " ")
 out s.findreplace("two" "R" 2 ",")
 out s.findreplace("two" "R" 2|0x200 ",")
 out s.findreplace("two" "R" 2 0)
 out s.findreplace("two" "R" 2 1)
 out s.findreplace("two" "R" 2 2)
 out s.findreplace("two, " "R" 2)
 out s.findreplace("two, " "R" 2|64)



out s
