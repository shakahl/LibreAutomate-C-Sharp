/// Use class <see cref="filesystem"/>.

/// Copy file or folder. Replace existing file(s).

string fc1 = @"C:\Test\file.txt"; //old path
string fc2 = @"C:\Test2\file.txt"; //new path
filesystem.copy(fc1, fc2, FIfExists.Delete);

filesystem.copyTo(fc1, @"C:\Test2"); //copy to the specified folder

//merge folders, replacing existing files
filesystem.copy(@"C:\Test", @"C:\Test2", FIfExists.MergeDirectory);

/// Copy all .png files to the specified folder. Don't include subfolders.

foreach (var f in filesystem.enumFiles(@"C:\Test", "*.png"))
	filesystem.copyTo(f.FullPath, @"C:\Test2");

//or can do the same with filter functions
filesystem.copy(@"C:\Test", @"C:\Test2", fileFilter: f => f.Name.Ends(".png", true), dirFilter: _ => 0);

/// Copy all .png files to the specified folder. From subfolders too.

//create subfolders like in the source folder
filesystem.copy(@"C:\Test", @"C:\Test2", 0, FCFlags.NoEmptyDirectories, f => f.Name.Ends(".png", true));

//copy all to single folder; give unique names if need
foreach (var f in filesystem.enumFiles(@"C:\Test", "*.png", FEFlags.AllDescendants))
	filesystem.copyTo(f.FullPath, @"C:\Test2", FIfExists.RenameNew);

/// The functions automatically create missing destination folders.
