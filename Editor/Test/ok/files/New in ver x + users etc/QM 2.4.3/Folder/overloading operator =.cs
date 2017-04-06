function Type1&a|Type2'b|intParam|$lpstrParam|... _type

sel _type
	case Type1 ;;support typename and type number (1 for Type1, 2 for Type2 ..., 0 if omitted param). The number could be used like if(_type<3) ... else ...
	...
	case Type2
	...

 Compiler at first compares the exact type of the passed value with Type1, Type2 and so on, until finds the match.
 If does not find a match, compares 1-level-unaliased type.
 If does not find a match, compares 2-level-unaliased type.
 If there are no more aliases, tries all possible auto-casts.


__________ OR ___________

function ?a $_typename

sel _typename
	case "int"
	...
	case "lpstr"
	...

 But then compiler cannot unalias. The function then must know all possible aliases. Not good.
