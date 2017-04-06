function $|str&s $rx [str&text|int&length|ARRAY(str)&|ARRAY(OFFSETS)&] [subMatch] [#from|FROMTO&] [#compFlags|REGEXOPTIONS&] ;;subMatch: 0, submatch index or name.  Returns match offset, or <0 if not found.
function lpstr|str&s $rx [str&|int&|ARRAY(str)&|ARRAY(OFFSETS)&|ARRAY(STRINT)&match] [subMatch] [int|FROMTO&ft] [int|REGEXOPTIONS&op] ;;subMatch: 0, submatch index or name.  Returns match offset, or <0 if not found.
function (lpstr|str&s) $rx [(str&|int&|ARRAY(str)&|ARRAY(OFFSETS)&|ARRAY(STRINT)&match)] [subMatch] [(int|FROMTO&ft)] [(int|REGEXOPTIONS&op)] ;;subMatch: 0, submatch index or name.  Returns match offset, or <0 if not found.

function lpstr'ls|str&s $rx [str&text|int&length|ARRAY(str)&as|ARRAY(OFFSETS)&ao|ARRAY(STRINT)&aso] [subMatch] [int'from|FROMTO&ft] [int'compFlags|REGEXOPTIONS&op] ;;subMatch: 0, submatch index or name.  Returns match offset, or <0 if not found.

sel _type1
	case 1 out ls
	case 2 out s

sel _type3
	case 0 out "omitted"
	case 1 out text
	 ...

 ...

 if only last param name given, then function can declare own variable and cast.
function lpstr|str&s
lpstr ls=+&s

 The parameters are in union. Or, if names given, they are normal parameters, else union.
 Because of union, cannot be composite types by value.

