function$ width [$repl] [$delim] [_flags] [minWidth] ;;_flags: 1 join lines, 2 leave spaces, 4 don't prefer spaces, 0x100 delim is table, 0x200 delim+blanks

 Word wrap.

 width - desired number of characters in lines. Minimum 4.
 repl - string to insert in place of line breaks. Default or "": "[]" (new-line).
 delim - <help #IDP_TABLEOFDELIM>delimiters</help>. The function always breaks at spaces and tabs, even if they are not included in delim. Characters from delim are used if there are no spaces in line, or flag 4 is used.
 _flags:
   1 - before processing join lines.
   2 - leave spaces around new line-breaks.
   4 - don't prefer spaces (all delimiter characters will have same priority).
   0x100 - delim is table of delimiters.
   0x200 - QM 2.3.3. Add blanks to delim.
 minWidth - minimal line width. Default: 0. For example, if you set minWidth equal to width, lines will be width length, regardless of delimiters.

 REMARKS
 Folds text into the specified width. Lines will never be longer than width characters. Breaks at spaces, tabs and characters specified in delim. If there are no spaces or other delimiter characters in the specified width, breaks word. Breaks lines by inserting new-line characters or repl string. Trims spaces around new line-breaks.
 Note that in Unicode mode text is interpreted as UTF-8, where non ASCII characters consist of several bytes. The function does not break a word in a middle of a multibyte character. width and _flags always is the number of bytes.


#if EXE=1
sub_sys.ExeQmPlusDll
#endif

qmplus_str_wrap &this width repl delim _flags minWidth
ret this

 note: delim also can be +1, +2. Before qm-2-4-1 would not need +.
