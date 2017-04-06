 Run this in QM:

_qmfile.FullSave

 Then QM file (and shared files) is in correct state and can be copied somewhere. Until that, recent changes are not written to it, they are in a temporary .wal file.

 Closing QM or hiding its window does the same. The Save button doesn't; it just saves to the .wal file.
