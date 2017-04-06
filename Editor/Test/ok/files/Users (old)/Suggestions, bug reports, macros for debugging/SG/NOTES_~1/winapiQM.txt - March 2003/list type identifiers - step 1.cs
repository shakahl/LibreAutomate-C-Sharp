 Macro was writen in March 2003

 Macro to check for errors in type declarations & use, in winapiQM.txt
  Check the final output (Step2) to find inconsistancies
	 eg.:
	 Rect change to -->  RECT {change required in many places)
 				-->Fixed in latest release
 				-->Ive run the macro and placed the results in "Check winapi" macro
	 type Size change to --> type SIZE (1 place)

 Step1 - gets and saves a list of all "type" names in winapiQM.txt
 Step2 - finds differences in use of upper/lower case in the rest of winapiQM.txt
	   - Output: last of: 1) name 2) list of each occurance in rest of file

str s t l tall folder loc ffound
if inp(folder "Path to winapiQM.txt?" "" "C:\Quick Macros 2\") = 0; end
ffound.searchpath(loc.from(folder "winapiQM.txt"))
if(!ffound) mes("%s not found." "" "" loc); end; else s.getfile(loc)
int x y a count
rep
	x = findw(s "type" x+1)
	if(x<0) break
	y = find(s " " x+5)
	t.get(s x+5 y-(x+5))
	tall+t
	tall+"[]"
	count+1
tall+count
loc.from(folder "type_names.txt")
tall.setfile(loc)
mes- "Type names copied to type_names.txt[][]     Continue to Step 2?" "" "OC"
mac "list type uses - step 2"


 	y = find(s "[]" x)
 	l.get(s x y-x)
 	t.gett(l 1 " ")
