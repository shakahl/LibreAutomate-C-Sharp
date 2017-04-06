function h m ins p1 p2

if(p1 = 0xfe) ret
 display messages:
 out "msg=0x%-10.8X    p1=0x%-10.8X    p2=0x%-10.8X" m p1 p2

 filter keyon messages:
if(m != 0x3c3 or p1 & 0xF0 != 0x90 or p1 & 0xff0000 = 0) ret

 get key:
str sk=p1 & 0xFF00 >> 8

 run menu:
mac _mt_menu sk
err act _hwndmt

 Function receives MIDI input messages and filters
 keyon messages. MIDI keys are numbered 0 to 127.
 When you press MIDI key with number that match menu
 item label, menu item is executed.
