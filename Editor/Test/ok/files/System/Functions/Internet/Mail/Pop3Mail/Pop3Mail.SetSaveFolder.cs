function $folder [flags] ;;flags: 1 delete old messages (eml files).

 Sets folder where to save messages.
 Creates if does not exist.

 REMARKS
 You should set folder before Save. Also, before Send with flag 0x800.


str s sf.expandpath(folder)
mkdir sf; err end _error
m_folder=sf

if(flags&1) del- s.from(sf "\*.eml"); err
