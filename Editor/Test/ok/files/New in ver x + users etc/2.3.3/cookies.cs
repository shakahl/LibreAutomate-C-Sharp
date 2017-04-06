function event $name [$newname]
 event: 1 added, 2 removed, 4 renamed, 8 modified
out "%i %s" event name

 sel event
	 case 8
	 sel name 3
		 case "*\Cookies\g@quickmacros[*].txt"
		  1
		 str s.getfile(name)
		 err ret ;;probably already deleted
		 out "---- %s ----" name
		 out s
		 out "--------------------------------"
