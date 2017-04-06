str s.getfile("Q:\Downloads\phpbb_posts.csv")
str rx=
 (?s)\[(code:\w+\]).+?\[/\1

 out s.len
out s.replacerx(rx)
 out s.len
s.setfile("Q:\Downloads\phpbb_posts.csv")
