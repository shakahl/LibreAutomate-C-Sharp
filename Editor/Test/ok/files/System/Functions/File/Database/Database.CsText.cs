function'str $folder [$moreParams]

 Creates connection string for text files (txt, csv, tab, asc).

 folder - parent folder.
 moreParams - will be appended to the connection string.

 REMARKS
 First row is used for column names.
 Use file name in sql as table name. Example: "Select * from customers.csv"


str s.format("Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=%s;Extensions=asc,csv,tab,txt;" _s.expandpath(folder))
s+moreParams

ret s
