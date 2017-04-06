ICsv icsvIn=CreateCsv()
icsvIn.Separator="[9]"
str s=
 one, two, three
 three, Treasure Scoop (Gem Scoop) 36" Handle, five

icsvIn.FromString(s)
out icsvIn.Cell(1 1)


icsvIn.Separator=","
icsvIn.ToString(_s); out _s
