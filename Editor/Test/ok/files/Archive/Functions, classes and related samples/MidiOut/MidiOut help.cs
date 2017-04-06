 Plays MIDI notes or sends other MIDI messages.

 EXAMPLE

#compile "__MidiOut"
MidiOut x.Open(-1)

x.SendB(0x90 60 127) ;;note 60 at max velocity
1
x.SendB(0x80 60) ;;stop note 60
x.SendB(0x90 50 127)
1
x.SendB(0x80 50)
