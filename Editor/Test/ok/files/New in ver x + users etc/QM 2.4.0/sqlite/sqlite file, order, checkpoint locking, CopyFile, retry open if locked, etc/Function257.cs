 \
function event $name [$newname]
 event: 1 added, 2 removed, 4 renamed, 8 modified
out F"{event} {name}"
sel event
	case 8
	Dir d
	if(d.dir(name))
		long t=d.TimeModified
		out _s.timeformat("{TT}" t)
