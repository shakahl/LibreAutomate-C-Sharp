function ARRAY(str)&an [ARRAY(str)&av] [flags] [$separator] ;;flags: 1 trim spaces in names

 Gets text of cells in a table in a web page.
 Most content in web pages is in tables, even if you don't see
 borders and grid. For example you can get list of links.

 This function cannot be used with tables whose cells contain other
 tables, because it gets only first level accessible objects in cells.

 This Acc variable must represent accessible object that is a child of the first cell you need. If you cannot capture the object using the Find Accessible Object dialog (for example its text is not known at run time), you can capture some other object and use post-navigation string.
 an - array that will be populated with accessible object names of this and subsequent cells. Can be 0 if not needed.
 av - array that will be populated with accessible object values of this and subsequent cells. Can be useful for example to get text of input boxes or URLs of links. Can be 0 if not needed.
 separator - string to be used to separate text of multiple objects in a cell (if the cell contains more than 1 object). Default: " ".

 EXAMPLE
 ARRAY(str) an
 Acc a=acc("Order #" "TEXT" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1801 0x40 0x20000040 "parent next6 first")
 a.WebTableToArray(an)
  display results
 out "------ results ------"
 int i
 for(i 0 an.len) out an[i]
 out "------ results with row and column numbers, assuming there are 7 columns ------"
 int r c nc=7
 for r 0 an.len/nc
	 out "--- row %i ---" r+1
	 for c 0 nc
		 out "column %i: %s" c+1 an[r*nc+c]


if(&an) an.redim
if(&av) av.redim
if(!len(separator)) separator=" "

Acc a2 a3
Navigate("parent" a2)
rep
	if(a2.Role=ROLE_SYSTEM_CELL)
		a2.Navigate("first" a3)
		if(&an)
			str& sn=an[an.redim(-1)]
			sn=a3.Name; if(flags&1) sn.trim
		if(&av)
			str& sv=av[av.redim(-1)]
			sv=a3.Value
		rep
			a3.Navigate("next"); err break
			if(&an)
				str s2=a3.Name; if(flags&1) s2.trim
				sn.from(sn separator s2)
			if(&av)
				sv.from(sv separator a3.Value)
	a2.Navigate("next"); err break
