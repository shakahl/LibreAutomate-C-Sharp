 Run this to test DDE server.
 At first run DDE_server_with_dialog.


Dde d.Connect("QM_dlg" "topic")

if(!d.Execute2("execute data")) out "Execute2 failed"

if(!d.Poke2("item" "poke data")) out "Poke2 failed"

str s
if(!d.Request2("item" s)) out "Request2 failed"
else out s
