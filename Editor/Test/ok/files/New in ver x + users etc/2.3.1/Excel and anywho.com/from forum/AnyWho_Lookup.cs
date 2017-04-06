function ARRAY(str)&ReturnInformation [str'First] [str'Last] [str'State] [str'City] [str'Zip] 

str AnyWhoURL.format("http://whitepages.anywho.com/results.php?&qc=%s&qf=%s&qn=%s&qs=%s&qz=%s&qi=0&qk=100" City First Last State Zip)

HtmlDoc d.InitFromWeb(AnyWhoURL)
str AnyWhoHTML=d.GetHtml
err
	str& Entry=ReturnInformation[];Entry.from("No Matches")
	ret

ARRAY(str) a ExactMatch Information
d.GetTable(12 a)
str Match=a
ARRAY(str) NumberOfResults

findrx(Match "Found (\d+)" 0 0 NumberOfResults)
int NumberMatch=val(NumberOfResults[1])
err 
	d.GetTable(13 ExactMatch)
	tok(ExactMatch[0] Information -1 "[]" 16)
	 out Information[0]
	err
		&Entry=ReturnInformation[];Entry.from("No Matches")
		ret
	&Entry=ReturnInformation[];Entry.from(Information[0])
	&Entry=ReturnInformation[];Entry.from(Information[1])
	&Entry=ReturnInformation[];Entry.from(Information[2])
	&Entry=ReturnInformation[];Entry.from(Information[3])
	ret

tok(Match Information -1 "[]" 16)
int i=3
if NumberMatch<100
	int RepNumber=NumberMatch
else
	RepNumber=100
rep RepNumber
	&Entry=ReturnInformation[];Entry.from(Information[i])
	&Entry=ReturnInformation[];Entry.from(Information[i+1])
	&Entry=ReturnInformation[];Entry.from(Information[i+2])
	&Entry=ReturnInformation[];Entry.from(Information[i+3])
	&Entry=ReturnInformation[];Entry.from("-------")
	i+10