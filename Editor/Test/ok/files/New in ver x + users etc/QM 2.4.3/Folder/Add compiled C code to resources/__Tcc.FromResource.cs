function'int* $resName

 Copies code and function addresses from a macro resource to an auto-managed memory.
 Returns pointer to the first address. Returns 0 if unavailable.

 resName - full resource name, as used with <help>str.getfile</help>. Examples: "resource:<>C_code", "resource:<MacroX>C_code".


lock

_s.getfile(resName); err ret
ret _CopyCode(+_s)
