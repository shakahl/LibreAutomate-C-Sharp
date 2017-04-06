 it seems cannot dump lib

 RunConsole2 "C:\Program Files\LLVM\bin\llvm-objdump.exe"
RunConsole2 "''C:\Program Files\LLVM\bin\llvm-objdump.exe'' -t ''Q:\SDK10\Lib\10.0.10586.0\um\x64\AdvAPI32.Lib''"

 USAGE: llvm-objdump.exe [options] <input object files>
 OPTIONS:
  -aarch64-neon-syntax    - Choose style of NEON code to emit from AArch64 backend:
    =generic              -   Emit generic NEON assembly
    =apple                -   Emit Apple-style NEON assembly
  -arch=<string>          - architecture(s) from a Mach-O file to dump
  -arch-name=<string>     - Target arch to disassemble for, see -version for available targets
  -archive-headers        - Print archive headers for Mach-O archives (requires -macho)
  -archive-member-offsets - Print the offset to each archive member for Mach-O archives (requires -macho and -archive-headers)
  -bind                   - Display mach-o binding info
  -color                  - use colored syntax highlighting (default=autodetect)
  -data-in-code           - Print the data in code table for Mach-O objects (requires -macho)
  -dis-symname=<string>   - disassemble just this symbol's instructions (requires -macho
  -disassemble            - Display assembler mnemonics for the machine instructions
  -disassemble-all        - Display assembler mnemonics for the machine instructions
  -dsym=<string>          - Use .dSYM file for debug info
  -dylib-id               - Print the shared library's id for the dylib Mach-O file (requires -macho)
  -dylibs-used            - Print the shared libraries used for linked Mach-O files (requires -macho)
  -exports-trie           - Display mach-o exported symbols
  -fault-map-section      - Display contents of faultmap section
  -full-leading-addr      - Print full leading address
  -g                      - Print line information from debug info if available
  -gpsize=<uint>          - Global Pointer Addressing Size.  The default size is 8.
  -help                   - Display available options (-help-hidden for more)
  -indirect-symbols       - Print indirect symbol table for Mach-O objects (requires -macho)
  -info-plist             - Print the info plist section as strings for Mach-O objects (requires -macho)
  -lazy-bind              - Display mach-o lazy binding info
  -link-opt-hints         - Print the linker optimization hints for Mach-O objects (requires -macho)
  -macho                  - Use MachO specific object file parser
  -mattr=<a1,+a2,-a3,...> - Target specific attributes
  -mcpu=<cpu-name>        - Target a specific cpu type (-mcpu=help for details)
  -mno-compound           - Disable looking for compound instructions for Hexagon
  -mno-pairing            - Disable looking for duplex instructions for Hexagon
  -no-leading-addr        - Print no leading address
  -no-show-raw-insn       - When disassembling instructions, do not print the instruction bytes.
  -no-symbolic-operands   - do not symbolic operands when disassembling (requires -macho)
  -non-verbose            - Print the info for Mach-O objects in non-verbose or numeric form (requires -macho)
  -objc-meta-data         - Print the Objective-C runtime meta data for Mach-O files (requires -macho)
  -print-imm-hex          - Use hex format for immediate values
  -private-header         - Display only the first format specific file header
  -private-headers        - Display format specific file headers
  -r                      - Display the relocation entries in the file
  -raw-clang-ast          - Dump the raw binary contents of the clang AST section
  -rebase                 - Display mach-o rebasing info
  -rng-seed=<seed>        - Seed for the random number generator
  -s                      - Display the content of each section
  -section=<string>       - Operate on the specified sections only. With -macho dump segment,section
  -section-headers        - Display summaries of the headers for each section.
  -stats                  - Enable statistics output from program (available with Asserts)
  -t                      - Display the symbol table
  -triple=<string>        - Target triple to disassemble for, see -version for available targets
  -universal-headers      - Print Mach-O universal headers (requires -macho)
  -unwind-info            - Display unwind information
  -version                - Display the version of this program
  -weak-bind              - Display mach-o weak binding info
  -x86-asm-syntax         - Choose style of code to emit from X86 backend:
    =att                  -   Emit AT&T-style assembly
    =intel                -   Emit Intel-style assembly
