 In Win2k+, keys can be disabled or mapped to other keys by adding value "Scancode Map" to
 "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout", and restarting QM.

 It is binary data. Format:
type SCANCODEMAP version flags nitems item[1]
 version and flags must be 0.
 nitems is number of items, including null terminator.
 items is array of items, one for each key, followed by 0.

 item format:
 0x??XX00YY, where XX is key scan code, and YY is scan code of key to map to, or 0 to disable.
 ?? probably can be 0, but in examples with Win key it is E0.

