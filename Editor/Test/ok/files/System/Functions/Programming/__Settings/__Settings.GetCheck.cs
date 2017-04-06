function! $name [column]

 Returns 1 if value is "Yes", or 0 otherwise.

 name - name.
 column - 0-based column index. Use when there are several columns of values.


ret !StrCompare(GetStr(name column) "Yes" 1)
