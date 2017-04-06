function $name ~value [column]

 Replaces value.

 name - name.
 value - value. Can be string or number.
 column - 0-based column index. Use when there are several columns of values.


if(!m_m.IntGet(name _i)) ret
m_c.Cell(_i column+2)=value
err+
