out
str f="$Common AppData$\__TestMyApp451"
del f; err
mkdir f
ChangeFileSecurity f "/E /G Users:F"
 ChangeFileSecurity f "/E /D Users"
 ChangeFileSecurity f "/E /G Users:W"
 ChangeFileSecurity f "/E /P Users:N"
 ChangeFileSecurity f "/E /P Users:C"
 ChangeFileSecurity f "/E /P Users:W"
 ChangeFileSecurity f "/E /P Users:R"
 ChangeFileSecurity f "/E /P Users:N"
 ChangeFileSecurity f "/E /R Users"
 ChangeFileSecurity f
 ChangeFileSecurity f "" _s; out _s
ChangeFileSecurity f "/E /P Users:R" _s; out _s

 run "$Common AppData$"
