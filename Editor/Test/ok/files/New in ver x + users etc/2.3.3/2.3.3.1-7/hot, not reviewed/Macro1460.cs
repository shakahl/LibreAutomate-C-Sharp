out
act "Feeling Good"
key Ca
str s.getsel
IStringMap m
GetWordsFromText s m

m.GetList(s)
out s
act _hwndqm
s.setfile("$documents$\words_fg.txt")
