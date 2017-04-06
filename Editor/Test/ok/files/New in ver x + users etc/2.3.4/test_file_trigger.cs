 \
function event $name [$newname]
 event: 1 added, 2 removed, 4 renamed, 8 modified

 Dir d
 if(!d.dir(name 1)) ret ;; obviously, it works only when a file is created

 out GetFileAttributes(name)

out "%i %s" event name
 
 out name