str qmlFile="$qm$\ok.qml"
 str qmlFile="$qm$\system.qml"
 str qmlFile="$my qm$\main.qml"

str db3File="$my qm$\test\ok.db3"

QmlToSqlite qmlFile db3File 0|0
 run db3File
 RunConsole2 F"Q:\Downloads\Contig.exe ''{_s.expandpath(db3File)}''"
