 Run this to test DDE server.
 At first run DDE_server.


Dde d.Connect("QM" "topic")

if(!d.Execute2("data")) out "Execute2 failed"

if(!d.Poke2("item" "data")) out "Poke2 failed"

str s
if(!d.Request2("item" s)) out "Request2 failed"
else out s
