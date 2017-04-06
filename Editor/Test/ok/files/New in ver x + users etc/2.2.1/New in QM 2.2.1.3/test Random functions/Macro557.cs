out "------------"
str s
RandomString s 8 8 ;;8 characters ASCII 33-127
out s

RandomString _s 8 16 "a-zA-Z0-9" ;;8 to 16 alphanumeric characters
out _s

out RandomString(_s 8 8 "[1]-[255]") ;;8 any characters
