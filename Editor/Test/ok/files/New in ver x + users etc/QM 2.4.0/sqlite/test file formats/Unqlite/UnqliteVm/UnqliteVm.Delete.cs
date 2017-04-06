
 Destroy unQLite virtual machine.
 Optional, called implicitly when the variable dies.
 Warning if fails.
 Calls unqlite_vm_release.


if(!m_vm) ret
if(__unqlite.unqlite_vm_release(m_vm)) end "failed to delete VM" 8|2
m_vm=0
