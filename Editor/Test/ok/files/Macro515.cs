_s=".fff."
_s.ReplaceInvalidFilenameCharacters
out _s

 str s="fff"
  s.setfile("Q:\test\.txt")
 out dir("Q:\test\.txt")
 out _s.getfile("Q:\test\.txt")
