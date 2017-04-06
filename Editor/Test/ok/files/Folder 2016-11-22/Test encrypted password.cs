 Change myEncryptionKey to match that in "Encrypted password generator".
 Also you can encrypt this macro.

str encryptedPassword="kkdodhfifpfpdkshgkd" ;;paste the result of macro "Encrypted password generator"

str password.decrypt(1|4 encryptedPassword "myEncryptionKey")
_s=
F
 ...
 smtp_password !{password}
 ...
SendMail "email" "subject" "text" 0 "" "" "" "" "" _s
