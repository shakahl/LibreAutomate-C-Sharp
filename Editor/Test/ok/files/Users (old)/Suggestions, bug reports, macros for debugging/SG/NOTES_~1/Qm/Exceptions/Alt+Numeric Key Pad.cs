 Alt+Numeric Key Pad
 ===================
 Neither of the methods below work to produce "æ"
 But if Alt+1+6+8 is done by hand, it works

 Wait, the results vary for first method - it may
 1) Work - If you click on the Run icon above
 2) Give Exception error
 3) If VK_NUMPAD1 (0x61) is a trigger for a macro, and you have another trigger (say F5)
    with a lef command to hit the Run icon above, then using F5 to launch this macro causes
    VK_NUMPAD1 to be keyed without Alt, which then launches the macro whose trigger is VK_NUMPAD1

 Wait, now after typing out the above it works ok...
 Results are inconsistant


 key A{(0x61)(0x66)(0x68)}

 key+ A
 key((0x61))
 key((0x66))
 key((0x68))
 key- A

 key# "æ"
 key A{N0N2N3N0}
key# "æ"
key# "[191]"

 