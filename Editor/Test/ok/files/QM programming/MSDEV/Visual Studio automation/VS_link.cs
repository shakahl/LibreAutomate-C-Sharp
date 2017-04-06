 /
function $fileLine [$linkText] [$textBefore] [$textAfter]

 Displays a link in QM output. It opens fileLine in Visual Studio 9 current window.

 fileLine - source file path. Can be with line, like "q:\app\app.cpp(200)".
 linkName - link text. If empty, uses fileLine.
 textBefore - text before linke, eg "<Z 0xC0ffff>".
 textAfter - text after link, eg "</Z>".


if(empty(linkText)) linkText=fileLine
out "<>%s<macro ''VS_goto /%s''>%s</macro>%s" textBefore fileLine linkText textAfter
