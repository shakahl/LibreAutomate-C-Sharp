out
act "Message"

str mySigFile="$desktop$\test.htm"
str s.getfile(mySigFile)
str sh.format("Version:1.0[]StartHTML:00000033[]%s" s)
sh.setsel("HTML Format")
