 /
function encrypt ~password [$macro]

 Automates encrypting or decrypting a macro or other QM item.

 encrypt - 1 to encrypt, 0 to decrypt.
 password - password.
 macro - name of the macro. If "" or omitted - macro that is currently open.

 EXAMPLES
 EncryptDecryptMacro 1 "password" ;;encrypt current macro
 EncryptDecryptMacro 0 "password" ;;decrypt current macro
 EncryptDecryptMacro 1 "password" "Dialog33" ;;encrypt Dialog33 


int w1=act(win("" "QM_Editor")) ;;activate QM
if(len(macro))
	int currentmacro=qmitem
	mac+ macro ;;open the macro, if specified
int w2=win("Options" "#32770" "qm") ;;is Options already open?
if(w2) ;;yes, remember that
	int optionsopen=1
else ;;no, open it
	men 2010 w1 ;;Options ...
	w2=wait(5 win("Options" "#32770" "qm"))
SelectTab id(12320 w2) 4 ;;select Security tab
password.setwintext(id(1051 w2)); password.setwintext(id(1053 w2)) ;;enter password
but iif(encrypt 1106 1107) w2 ;;click Encrypt or Decrypt button
if(!optionsopen) but 2 w2; wait 10 -WC w2 ;;close
if(currentmacro) mac+ currentmacro ;;open current macro again

err+ end _error
