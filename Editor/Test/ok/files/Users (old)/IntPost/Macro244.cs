  posts form with file field (userfile)
str s sr
s.getfile("$desktop$\test.txt")
 s.getfile("$qm$\winapiqm.txt")
s-"--7d23542a1a12c2[]Content-Disposition: form-data; name=''userfile''; filename=''test.txt''[]Content-Type: text/plain[][]"
s+"[]--7d23542a1a12c2--[]"
IntPost "http://www.nightchatter.com/uploader.php" s sr "Content-Type: multipart/form-data; boundary=7d23542a1a12c2"
out sr
