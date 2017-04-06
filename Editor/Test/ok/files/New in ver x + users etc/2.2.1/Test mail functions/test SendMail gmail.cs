 note: fails if Avast on. Disable SSL in Avast Mail Shield settings.

_s=
 smtp_server smtp.gmail.com
 smtp_port 465
 smtp_user qmgindi@gmail.com
 smtp_password 3318B2C945B213CB582D8C1FF577859A05
 smtp_auth 2
 smtp_secure 1
 smtp_timeout 30
 smtp_email qmgindi@gmail.com
 smtp_displayname "Gintaras Didzgalvis"
 smtp_replyto ""
SendMail "G <support@quickmacros.com>" "s2" "s2" 0x80000 "" "" "" "" "" _s
 err run "$my qm$\smtp_log.txt"


 SendMail "G <support@quickmacros.com>" "s2" "s2" 0 "" "" "" "" "" "qmgindi@gmail.com"
 smtp_server smtp.gmail.com
 smtp_port 465
 smtp_user qmgindi@gmail.com
 smtp_password 3318B2C945B213CB582D8C1FF577859A05
 smtp_auth 2
 smtp_secure 1
 smtp_timeout 120
 smtp_email qmgindi@gmail.com
 smtp_displayname gd
 smtp_replyto ""

