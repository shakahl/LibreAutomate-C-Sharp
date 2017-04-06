function $fileLine

 Opens fileLine in Visual Studio 9 (in currently active solution).
 If fileLine is like "file(line)", scrolls to the line.
 If fileLine empty, uses _command.
 Works async, because runs CVisualStudio_GoTo.qmm as User.


if(empty(fileLine)) fileLine=_command
mac "CVisualStudio_GoTo" fileLine
