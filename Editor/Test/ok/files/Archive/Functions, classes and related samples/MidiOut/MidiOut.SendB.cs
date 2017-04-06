function byte1 [byte2] [byte3]

 Sends short MIDI message.
 Error if fails.
 For more information look for midiOutShortMsg in MSDN.

 byte1, byte2, byte3 - status byte and data bytes. When a series of messages have the same status byte, byte1 and byte2 are data bytes, and byte3 not used.


SendI(byte1|(byte2<<8)|(byte3<<16))
