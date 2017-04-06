 Copies mp3 files specified in "$documents$\friend.m3u8"
 to "$personal$\frimu".
 Deletes old mp3 files from the destination folder.

str s sf dest
sf.getfile("$documents$\friend.m3u8")
dest="$personal$\frimu"
mkdir dest

del- _s.from(dest "\*.mp3")

foreach s sf
	if s.begi("Q:")
		cop s dest

run dest
