ref  __unqlite "Q:\app\unqlite.txt" 1   ;;UnQLite API. Used by Unqlite class. Added in QM 2.3.7.
class Unqlite -!*m_db ;;QM 2.3.7. Unqlite database functions. Wraps unqlite* C object.
class UnqliteVm -!*m_vm ;;QM 2.3.7. Compiles and executes Unqlite database script. Wraps unqlite_vm* C object.
class UnqliteCursor unqlite_kv_cursor*c -Unqlite*m_db
