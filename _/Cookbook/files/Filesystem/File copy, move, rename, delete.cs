/// Use class <see cref="filesystem"/>.

/// Copy file or folder. Specify full path of the destination. Delete the destination if exists.

string f1 = @"C:\Test\file.txt";
string f2 = @"C:\Test2\file.txt";
filesystem.copy(f1, f2, FIfExists.Delete);

/// Copy file or folder to the specified folder.

filesystem.copyTo(f1, @"C:\Test2");

/// Copy all .png files to another folder.

filesystem.copyTo(@"C:\Test", @"C:\Test2", filter: k => k.IsDirectory || k.Name.Ends(".png", true));
