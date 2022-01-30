/// Delete a file or folder; move to the Recycle Bin if possible.

filesystem.delete(@"Q:\Test\file.txt", recycleBin: true);

/// Empty the Recycle Bin.

computer.emptyRecycleBin(); //all drives
computer.emptyRecycleBin("C:"); //single drive
computer.emptyRecycleBin(progressUI: true); //with progress UI

/// Open the RecycleBin folder.

run.it(folders.shell.RecycleBin);
