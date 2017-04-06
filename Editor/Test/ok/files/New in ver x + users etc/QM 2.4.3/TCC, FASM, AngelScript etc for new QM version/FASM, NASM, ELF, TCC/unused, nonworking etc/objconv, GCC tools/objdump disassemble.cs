 Useless.

out
str prog="C:\MinGW\bin\objdump.exe"
 RunConsole2 F"{prog}"

RunConsole2 F"{prog} --disassemble Q:\app\Release\TestTccLink.obj"
 RunConsole2 F"{prog} -fnasm Q:\app\qmcore\pcre\Release\pcre.obj Q:\Downloads\objconv\pcre.s"
 RunConsole2 F"{prog} -fnasm Q:\app\Release\TestTccLink.obj Q:\Downloads\objconv\TestTccLink.s"

 str s.getfile("Q:\Downloads\objconv\pcre.s")
 out s.replacerx("\$(\w)" "_1_$1")
 out s.replacerx("\?\?(\w)" "_2_$1")
 out s.replacerx("(\w)@" "$1_3_")
 out s.replacerx("(\w)\?" "$1_4_")
 s.setfile("Q:\Downloads\objconv\pcre.s")


#ret
Usage: C:\MinGW\bin\objdump.exe <option(s)> <file(s)>
 Display information from object <file(s)>.
 At least one of the following switches must be given:
  -a, --archive-headers    Display archive header information
  -f, --file-headers       Display the contents of the overall file header
  -p, --private-headers    Display object format specific file header contents
  -P, --private=OPT,OPT... Display object format specific contents
  -h, --[section-]headers  Display the contents of the section headers
  -x, --all-headers        Display the contents of all headers
  -d, --disassemble        Display assembler contents of executable sections
  -D, --disassemble-all    Display assembler contents of all sections
  -S, --source             Intermix source code with disassembly
  -s, --full-contents      Display the full contents of all sections requested
  -g, --debugging          Display debug information in object file
  -e, --debugging-tags     Display debug information using ctags style
  -G, --stabs              Display (in raw form) any STABS info in the file
  -W[lLiaprmfFsoRt] or
  --dwarf[=rawline,=decodedline,=info,=abbrev,=pubnames,=aranges,=macro,=frames,
          =frames-interp,=str,=loc,=Ranges,=pubtypes,
          =gdb_index,=trace_info,=trace_abbrev,=trace_aranges,
          =addr,=cu_index]
                           Display DWARF info in the file
  -t, --syms               Display the contents of the symbol table(s)
  -T, --dynamic-syms       Display the contents of the dynamic symbol table
  -r, --reloc              Display the relocation entries in the file
  -R, --dynamic-reloc      Display the dynamic relocation entries in the file
  @<file>                  Read options from <file>
  -v, --version            Display this program's version number
  -i, --info               List object formats and architectures supported
  -H, --help               Display this information
