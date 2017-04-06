function Unqlite&db $code [flags] ;;flags: 1 code is file

 Compile a Jx9 script (unqlite_compile) and initializes this variable.
 Error if fails, eg if there are errors in script.
 Calls unqlite_compile or unqlite_compile_file.


opt noerrorshere 1; opt nowarningshere 1
Delete
int e
if(flags&1) e=__unqlite.unqlite_compile_file(db _s.expandpath(code) &m_vm)
else e=__unqlite.unqlite_compile(db code -1 &m_vm)
if(e) db.E(1)

out __unqlite.unqlite_vm_config(m_vm __unqlite.UNQLITE_VM_CONFIG_OUTPUT &UnqliteVm_Output 0)
