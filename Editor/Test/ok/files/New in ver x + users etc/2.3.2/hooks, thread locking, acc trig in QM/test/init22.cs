
str+ _sfnName
def fnName _sfnName.getmacro(getopt(itemid) 1)
def nowstr _s.time("" "hh:mm:ss")
def tabchar "[9]"


out "%s init2:" nowstr


 Run when Windows starts, disable BlockInput2 
 BlockInput2 0 ;; Managed to lock myself out of the PC when testing BlocInput2! Or was I confused?

def doubleKeyPressDelayMS 400 ;; Milli-seconds that a double keypress must occur within


 CompileAllItems "MIKeyboardProc"

; Added with Tools / Type Libraries
 typelib Word {00020905-0000-0000-C000-000000000046} 8.0
 typelib Outlook {00062FFF-0000-0000-C000-000000000046} 9.2


; In initialise some functions
openNextExe "" ""
