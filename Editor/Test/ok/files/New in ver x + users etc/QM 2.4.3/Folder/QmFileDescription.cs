 /
function $qmlFile str&description

 Gets .qml file description.

 EXAMPLE
 str description
 QmFileDescription "$my qm$\file.qml" description
 out description


opt noerrorshere 1
int retry
ARRAY(str) a
 g1
Sqlite x.Open(qmlFile 1)
x.Exec("SELECT text FROM texts WHERE rowid=0" a)
err ;;eror if the file is open. Copy it to a temp file.
	if(retry) end _error
	retry=1
	_qmfile.FullSave
	__TempFile tf.Init
	cop- qmlFile tf
	qmlFile=tf
	goto g1
x.Close
description=a[0 0]
