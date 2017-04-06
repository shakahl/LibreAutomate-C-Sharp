function$ $name [column]

 Gets value as string.

 name - name.
 column - 0-based column index. Use when there are several columns of values.

 REMARKS
 The returned string is temporary. To be safe, assign to a str variable.


if(!m_m.IntGet(name _i)) ret
ret m_c.Cell(_i column+2)
err+
