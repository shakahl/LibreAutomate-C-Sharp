 This code until ------ creates files for testing. Delete it.
 Select the following text inside "" and run this macro.
 "08-05 Smith - Johnson -"
 It should display 3 files.

out

 str files=
  C:\Quotes\2014\08-05 Smith - Johnson - Gen $4500 Monthly 6yrs Shared 3% Comp.pdf
  C:\Quotes\2014\08-05 Smith  -  Johnson  -  Comparison.pdf
  C:\Quotes\2014\08-05 Smith-Johnson-Thrivent $4500 Monthly 6yrs Shared 3% Comp.pdf
  C:\Quotes\2014\08-05 Somebody - Else - Gen $5000 Monthly 8yrs Shared 3% Comp.pdf
  C:\Quotes\2014\08-05 Somebody - Else - Comparison.pdf
 
 mkdir "C:\Quotes\2014"
 str _s1 _s2="test"
 foreach(_s1 files) _s2.setfile(_s1)

 --------------

str s.getsel
str attachmentsList

str rx=" *- *"
s.replacerx(rx "-")
 out s

ARRAY(str) a; int i
GetFilesInFolder a "C:\Quotes\2014" "*.pdf"
for i 0 a.len
	 out "----"; out a[i]
	str s2
	if(findrx(a[i] "\\Quotes\\20\d\d\\(\d\d-\d\d [^-]+-[^-]+-).+\.pdf" 0 1 s2 1)<0) end F"incorrect filename format: {a[i]}"
	s2.replacerx(rx "-")
	 out s2
	if(!(s2~s)) continue
	 out a[i]
	attachmentsList.addline(a[i])

out attachmentsList
