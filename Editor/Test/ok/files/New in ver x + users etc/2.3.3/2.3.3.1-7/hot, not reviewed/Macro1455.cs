out
str s.getfile("$desktop$\words.txt")

str s2 s3
foreach s2 s
	s3=s2; s3.stem
	if(s3.end("i")) out F"{s2} {s3}"
