str qmlFile="$qm$\ok.qml"
str qmcFile="$my qm$\test\ok.qmc"

QmlToCompound qmlFile qmcFile

out GetFileOrFolderSize(qmcFile)/1024
 out GetFileFragmentation(qmcFile)
RunConsole2 F"Q:\Downloads\Contig.exe ''{_s.expandpath(qmcFile)}''"
