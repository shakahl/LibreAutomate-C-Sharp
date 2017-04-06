 this is for testing
 Htm elt=htm("INPUT" "submit" "" win("Quick Macros Forum" "IEFrame") 0 1 0x521)
 elt.SetText("Find Peter")
 elt.Mouse

 ________________________


Htm el=htm("INPUT" "Find Peter" "" win("Quick Macros Forum" "IEFrame") 0 1 0x421)
str s var1 f
s=el.Text
var1="a"
f.from("Find " var1)
el.SetText(f)
