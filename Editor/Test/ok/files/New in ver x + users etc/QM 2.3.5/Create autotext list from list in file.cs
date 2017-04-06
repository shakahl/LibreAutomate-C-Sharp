out
 str s.getfile("...")
str s=
 Bach: Jean-Sebastien Bach
 Beethoven: Ludwig van Beethoven
 Bruckner: Anton Bruckner
 Mahler: gustav Mahler
 Monteverdi: claudio Monteverdi
 Mozart: Wolfgang amadeus Mozart

if(!s.replacerx("^(\w)(.+?): *([^\r\n]+)" "$1 :''$3'' ;;$1$2" 9)) end "incorrect list"
 out s
s-"/b/i/c[]"
s.setmacro("Autotext")
