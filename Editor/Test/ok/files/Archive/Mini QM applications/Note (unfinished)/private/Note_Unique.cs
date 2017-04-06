function~ $name

str s ss su; int i
mkdir s.expandpath("$personal$\Notes")

for(i 1 1000000000)
	if(!dir(ss.format("%s\%s%i.txt" s name i) iif(i=1 0 3)))
		ss.from(name i)
		su.ucase(ss)
		if(!win(su "QM_toolbar")) ret ss
	