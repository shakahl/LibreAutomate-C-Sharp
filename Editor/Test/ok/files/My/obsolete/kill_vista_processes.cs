function event pid pidparent $name
 event: 1 started, 2 ended, 4 running
sel name 1
	 case ["trustedinstaller","searchindexer"]
	case "searchindexer"
	ShutDownProcess pid 1
	err out "%s: %s" name _error.description; ret
	out "killed %s" name
	