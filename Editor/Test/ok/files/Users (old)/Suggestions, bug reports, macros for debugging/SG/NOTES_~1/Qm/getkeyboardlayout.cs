int s
long k2
lpstr k="                          "
 k.all(20)
GetKeyboardLayoutName(k);;00020409;;09=Primary Lang Identifier=English
out k

 NOTE: if change layout temporarily (using Left-Alt-Shift), still get the default
      but OK if change layout through control panel

   -So how to know what keyboard layout user is currently using?
    Not what the default is, according to the control panel, 
    but what user is actively using at the current moment

k2= GetKeyboardLayout(0);;f0020436;;Language Identifier 0436=Afrikaans
out "%x" k2                       ;;Primary Lang Id=36=Afrikaans

out "%x" GetSystemDefaultLangID();;81840409
out "%x" GetUserDefaultLangID()



 0x0409 - U.S. Language Identifier
 00000409 - U.S. English Layout
 00010409, 00020409 - U.S. English Dvorak

 Output for U.S. English
 00000409
 4090409
 1210409
 1210409

 Output for U.S. English Dvorak
 00020409
 f0020406
 1210409
 1210409
