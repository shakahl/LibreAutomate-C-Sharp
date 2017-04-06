 No precedence at all.
 Instead, don't allow multiple operators in expression without ().
r=a+b*c ;;error
r=a+(b*c) ;;ok

 Benefits:
 Don't need to learn/remember operator precedence.
 Easier to read (no doubts).
 If used for prototyping, will not need to edit, whatever is the target language. Because different languages have different operator precedence, and in most languages it is not optimal.
 Maybe it could be an option. Else use 5 priority classes for binary operators: */%, +-, bitwise, compare, logical.
 Or even allow to define presets of operator priorities and other language properties, eg use or not commas between parameters, always require () for parameters, use {} or indentation, comments // or ;;, use C-style switch/for/etc.
