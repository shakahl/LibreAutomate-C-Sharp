ICsv icsvIn=CreateCsv()
icsvIn.Separator=","
str s=
 one, two, three
 three, "four""", five

icsvIn.FromString(s)
out icsvIn.Cell(1 1)


icsvIn.Separator=","
icsvIn.ToString(_s); out _s
