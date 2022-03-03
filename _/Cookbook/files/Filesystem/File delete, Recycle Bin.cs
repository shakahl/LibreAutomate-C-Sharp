/// Use class <see cref="filesystem"/>.

/// Delete file or folder.

filesystem.delete(@"C:\Test2\file.txt"); //delete permanantly
filesystem.delete(@"C:\Test2\file.txt", recycleBin: true); //move to the Recycle Bin if possible, else delete permanently

/// Delete everything from folder (make it empty).

foreach (var f in filesystem.enumerate(@"C:\Test2"))
	filesystem.delete(f.FullPath);

/// Delete all .txt files from folder. Not from subfolders.

foreach (var f in filesystem.enumFiles(@"C:\Test", "*.txt"))
	filesystem.delete(f.FullPath);

/// Empty the Recycle Bin.

filesystem.more.emptyRecycleBin(); //all drives
filesystem.more.emptyRecycleBin("C:"); //single drive
filesystem.more.emptyRecycleBin(progressUI: true); //with progress UI

/// Open the Recycle Bin folder.

run.it(folders.shell.RecycleBin);
