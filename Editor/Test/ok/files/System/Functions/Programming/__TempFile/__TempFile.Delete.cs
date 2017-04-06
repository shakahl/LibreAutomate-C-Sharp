
 Deletes the temporary file (if exists), and clears this variable.
 If fails to delete, just displays warning in QM output.
 This function is implicitly called when destroying this variable.


if(FileExists(m_file 2)) FileDelete m_file; err end F"Failed to delete temporary file {m_file}" 16|8
m_file.all
