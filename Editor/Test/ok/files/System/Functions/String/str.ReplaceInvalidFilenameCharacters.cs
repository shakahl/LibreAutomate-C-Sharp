function [$replaceto]

 Replaces or removes characters that cannot be used in file names.

 REMARKS
 <google>Windows file and path naming conventions</google>

 This function ignores string length. Normally max path length is 259; max filename length is less.

 QM 2.4.4: Also corrects other forms of invalid or problematic filename (except invalid length):
   Trims spaces and other blank characters.
   Replaces "." at the end.
   Prepends "@" if a reserved name like "CON" or "CON.txt".

 EXAMPLE
 str s="file*name.txt "
 s.ReplaceInvalidFilenameCharacters("_")
 out F"''{s}''"


trim
if(this.len and replacerx("\.$|[\\/|<>?*:''[1]-[31]]" replaceto)) trim
if(0=findrx(this "^(CON|PRN|AUX|NUL|COM\d|LPT\d)(\.|$)" 0 1)) this-"@"
