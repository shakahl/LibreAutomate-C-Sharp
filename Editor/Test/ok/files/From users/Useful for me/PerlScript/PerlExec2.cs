 /
function str'code

 Executes PerlScript code. Similar to PerlScript, but this function saves the code to a temporary .pl file,
 which is executed by the WSH engine, that is, not in QM context.


str sf="$my qm$\PerlExec2.pls"
code.setfile(sf)
run sf
