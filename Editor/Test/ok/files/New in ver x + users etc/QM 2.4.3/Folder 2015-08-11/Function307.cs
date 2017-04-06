\
function event pid pidparent $name
 event: 1 started, 2 ended, 4 running
DateTime t.FromComputerTime
out F"{t.ToStr(2|8)} {event} {pid} {name}"

 sel name
	 case "backgroundTaskHost"
	 ShutDownProcess pid 1
