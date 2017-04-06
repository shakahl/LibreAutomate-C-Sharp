function$ [_flags] [_from] [wordLen]) ;;_flags: 1 all, 2 spaces.

 Gets stem of word(s).

 _flags:
   1 - replace all words. The function breaks words assuming that word characters are a-z, A-Z, 0-9, _.
   2 - when used with flag 1, replace all nonword characters with spaces.
 _from, wordLen - word offset and length in the string. If used and >0, replaces just this part of the string.

 REMARKS
 Removes suffix from a word. For example, "worked", "working" and similar word forms are replaced with "work". Supports only English.
 Stemming usually is used to find words that may be in any form, for example "work", "works", "worked", "working". For example, Google uses it.
 The function is not to get exact root. It is to get common word form for searching. The result may be shorter, or not exactly match an existing word. For example, "happy" and "happiness" are replaced with "happi". The function usually does not change parts of speech, e.g. "worker" (noun) to "work" (verb). The function is not perfect. For example, it does not replace "found" with "find". It also does not remove prefixes.
 
 Unless flag 1 used, this variable should contain single word, or it can contain multiple words and the word place is specified using _from and wordLen.
 When using _from and wordLen, if the word is not at the end, the function replaces the suffix with space characters. If there was 0 (null) character at the end of the word (for example, after tok), instead replaces the suffix with 0 characters.
 
 The function also makes the word lowercase, regardless of whether there was a suffix.

 Added in: QM 2.2.1.

 EXAMPLES
 str s="working"
 s.stem
 out s ;;"work"
 
 s="THEY WORKED WELL"
 s.stem(0 5 6)
 out s ;;"THEY work   WELL"
 
 str s="it worked well"
 str s2="works"
 s.stem(1|2)
 s2.stem
 int i=findw(s s2)
 out i


#if EXE=1
sub_sys.ExeQmPlusDll
#endif

qmplus_str_stem &this _flags _from wordLen
ret this
