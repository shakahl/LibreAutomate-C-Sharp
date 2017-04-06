function# $name

 Finds column in SELECT results.
 Returns its 0-based index.
 Error if not found.


int i
for i 0 __sqlite.sqlite3_column_count(p)
	if(!StrCompare(name __sqlite.sqlite3_column_name(p i))) ret i
end F"column {name} does not exist in results" 2

 info: caching would not make much faster
