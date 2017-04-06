 /
function $qmlFileToRepair $sqliteExe

 Repairs/fixes a corrupt QM file, if possible.

 qmlFileToRepair - the corrupt QM file (.qml).
   This function does not replace the file. It creates new (repaired) file with unique name in the same folder.
   The file (qmlFileToRepair) must not be currently loaded in QM. To call this function, create or open other QM file.
 sqliteExe - sqlite3.exe full path. This function executes it to extract valid data from qmlFileToRepair and create new file.
   Download sqlite-shell-win32-x86-nnnnnnnn.zip from <link>https://sqlite.org/download.html</link>.

 REMARKS
 QM stores macros and other data in .qml files that are <link "www.sqlite.org">SQLite</link> databases (QM 2.4.0).
 It is possible to <link "www.sqlite.org/howtocorrupt.html">corrupt</link> a SQLite database. Then you may see error "database disk image is malformed" when QM starts or later.
 Usually the best way to fix it - replace the file with the newest backup file. You can find backup files in Options -> Files.
 Or you can try to repair the file. Open or create other QM file and create/run macro that calls this function. At first download sqlite3.exe (see above).
 If this function fails, it either generates RT error or shows errors in QM output, however may anyway create file with data that was possible to extract. If succeeds, shows several lines in QM output.
 If this runs successfully, replace the corrupt file with the file that this function creates. Open it again in QM.
 Note that some data may be missing or changed, especially if this function shows some errors.
 Note that the new file may be smaller, even if all data successfully extracted. It is because SQLite database files may have some unused space for new data to insert faster.

 EXAMPLE
 RepairQmFile "$my qm$\main.qml" "$qm$\sqlite3.exe"


opt noerrorshere 1

str file1.expandpath(qmlFileToRepair)
str file2=file1; file2.UniqueFileName ;;-init fails if file2 exists
__TempFile fileSQL.expandpath("$temp qm$\RepairQmFile.sql")
str sqlite3.expandpath(sqliteExe)

system F"echo .dump | ''{sqlite3}'' ''{file1}'' > ''{fileSQL}''" ;;dump valid data to SQL text file
RunConsole2 F"{sqlite3} -init ''{fileSQL}'' ''{file2}''" ;;recreate database from SQL
_s.getfile(file1 60 12); _s.setfile(file2 60 12 4) ;;copy QM application_id and user_version
_s.getfile(file1 18 2); _s.setfile(file2 18 2 4) ;;copy WAL mode
