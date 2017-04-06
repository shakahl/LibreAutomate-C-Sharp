str qmlFile="$qm$\ok.qml"
str qmcFile="$my qm$\ok.qmc"

 str qmlFile="$my qm$\main.qml"
 str db3File="$my qm$\main.qmc"

QmlToQmf qmlFile qmcFile

out GetFileOrFolderSize(qmcFile)/1024
RunConsole2 F"Q:\Downloads\Contig.exe -a ''{_s.expandpath(qmcFile)}''"
