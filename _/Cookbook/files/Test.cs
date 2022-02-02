/*/ role miniProgram; /*/

//filesystem.copy(@"C:\Test", @"C:\Test2", filter: k => k.Name.Ends(".png", true));
//filesystem.copy(@"C:\Test", @"C:\Test2", filter: k => k.IsDirectory || k.Name.Ends(".png", true));

print.clear();
//print.it(filesystem.enumerate(@"C:\Test", FEFlags.AndSubdirectories, filter: k => k.Name.Ends(".png", true)));
//print.it(filesystem.enumerate(@"C:\Test", FEFlags.AndSubdirectories, filter: k => k.Name.Ends(".png", true)));
//print.it(filesystem.enumerate(@"C:\Test", FEFlags.AndSubdirectories | FEFlags.OnlyFiles));
//print.it(filesystem.enumerate(@"C:\Test", FEFlags.AndSubdirectories | FEFlags.OnlyFiles, k => k.IsDirectory || k.Name.Ends(".png", true)));
//print.it(filesystem.enumerate(@"C:\Test", FEFlags.AndSubdirectories, k => k.DirOnlyChildren() || k.Name.Ends(".png", true)));
//print.it(filesystem.enumerate(@"C:\Test", "*.png", FEFlags.AndSubdirectories);

//print.it(Directory.EnumerateFiles(@"C:\Test", "*.png", new EnumerationOptions() { RecurseSubdirectories = true }));
//print.it(Directory.EnumerateFiles(@"C:\Test", "*", new EnumerationOptions() { RecurseSubdirectories = true }));
//print.it(Directory.EnumerateFiles(@"C:\Test", "*.png", new EnumerationOptions() {  }));
//print.it(Directory.EnumerateFiles(@"C:\Test\123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_\123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_\123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_123456789_Z", "*", new EnumerationOptions() {  }));
//print.it(Directory.EnumerateFiles(@"C:\Test", "*.png"));
//print.it(Directory.EnumerateDirectories(@"C:\Test", "*", new EnumerationOptions() { RecurseSubdirectories = true }));
//print.it(Directory.EnumerateFileSystemEntries(@"C:\Test", "*", new EnumerationOptions() { RecurseSubdirectories = true }));
//print.it(Directory.GetFiles(@"C:\Test", "*", new EnumerationOptions() { RecurseSubdirectories = true }));
//print.it(Directory.EnumerateFileSystemEntries(@"C:\", "*", new EnumerationOptions() {  }));
//print.it(new DirectoryInfo(@"C:\Test").EnumerateFiles("*", SearchOption.AllDirectories));

//enum FEFilter {
//	This,
//	Children,
	
//}

//FEFilter f;
//f = k => k.IsDirectory ? FEFilter.Children : k.Name.Ends(".png", true) ? FEFilter.This : 0;

//f = k => k.DirChildren() || k.Name.Ends(".png", true);

//f = k => k.DirOnly() || k.Name.Ends(".png", true);

//f = k => k.IsFile("*.png");

var dir = @"C:\Windows\System32";
//dir = @"C:\Program Files";

1.s();
perf.first();
int n=0;
foreach (var f in new DirectoryInfo(dir).EnumerateFileSystemInfos("*", new EnumerationOptions { RecurseSubdirectories = true, AttributesToSkip = 0 })) {
	n++;
}
//foreach (var f in Directory.EnumerateFileSystemEntries(@"C:\Windows\System32", "*", new EnumerationOptions { RecurseSubdirectories = true, AttributesToSkip = 0 })) {
//	n++;
//}
//foreach (var f in Directory.GetFiles(dir, "*", new EnumerationOptions { RecurseSubdirectories = true, AttributesToSkip = 0 })) {
//	n++;
//}
perf.nw();
print.it(n);
1.s();
perf.first();
int nn=0;
foreach (var f in filesystem.enumerate(dir, FEFlags.AndSubdirectories | FEFlags.IgnoreInaccessible)) {
	nn++;
}
perf.nw();
print.it(nn);
