 \
function $s

 Displays s in Visual Studio 9 output window (in currently active solution).
 If it is like "file(line)", you can double click to jump to the line. Or can be "file(line) : some text".
 If s empty, uses _command.


if(empty(s)) s=_command
#compile "__CVisualStudio"
CVisualStudio x.Out(s)
err end _error
