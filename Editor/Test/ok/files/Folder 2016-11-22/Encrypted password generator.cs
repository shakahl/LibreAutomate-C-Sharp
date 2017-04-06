 Change myEncryptionKey.
 Also you can encrypt this macro.

 g1
str password
if(!inp(password "Encrypted password will be stored in the clipboard." "Password" "*")) ret
if(password.len<6) mes "min 6 characters" "" "i"; goto g1
str encryptedPassword.encrypt(1|4 password "myEncryptionKey")
encryptedPassword.setclip
