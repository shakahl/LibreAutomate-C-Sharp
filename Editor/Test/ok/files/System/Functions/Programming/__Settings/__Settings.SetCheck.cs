function $name check [column]

 Replaces value of a checkbox cell.

 name - name.
 value - if nonzero, sets value "Yes", else sets empty.
 column - 0-based column index. Use when there are several columns of values.


if(!m_m.IntGet(name _i)) ret
m_c.Cell(_i column+2)=iif(check "Yes" "")
err+
