str s="Ąčę Ėįš Šųū"
out find(s "Ėįš")
out find(s "ĖĮš" 0 1)
 s.findreplace("Ėįš" "-")
s.findreplace("ĖĮš" "-" 1)
out s
