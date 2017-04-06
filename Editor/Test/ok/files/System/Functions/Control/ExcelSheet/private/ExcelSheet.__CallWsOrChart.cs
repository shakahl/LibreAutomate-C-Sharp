function IDispatch&d f [str&name] ;;f: 0 get name, 1 set name, 2 delete, 3 activate

 Calls a Worksheet or Chart function.
 Cannot call through IDispatch because of Excel bug:
   Sometimes for some sheets error "unknown member".
   Fails GetIdsOfNames with an unknown error.
   Same with VBScript.


Excel.Worksheet w
Excel.Chart c

w=d; err c=d; err

if w
	sel f
		case 0 name=w.Name
		case 1 w.Name=name
		case 2 w.Delete
		case 3 w.Activate
else if c
	sel f
		case 0 name=c.Name
		case 1 c.Name=name
		case 2 c.Delete
		case 3 c.Activate
else
	sel f
		case 0 name=d.Name
		case 1 d.Name=name
		case 2 d.Delete
		case 3 d.Activate

err+ end _error
