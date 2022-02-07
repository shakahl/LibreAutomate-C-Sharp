/// Use classes from namespace <see cref="System.IO.Compression"/>.

string zipFile = @"Q:\Test\Z.zip";
//filesystem.delete(zipFile);

/// Create .zip file from folder.

ZipFile.CreateFromDirectory(@"Q:\Test\Folder", zipFile);
ZipFile.CreateFromDirectory(@"Q:\Test\Folder", zipFile, CompressionLevel.Fastest, includeBaseDirectory: true);

/// Extract .zip file.

ZipFile.ExtractToDirectory(zipFile, @"Q:\Test\Folder2");

/// Create .zip file from single file.

string file = @"Q:\Test\sqlite.db";
using (var z = ZipFile.Open(zipFile, ZipArchiveMode.Create))
	z.CreateEntryFromFile(file, pathname.getName(file));

/// Create .zip file and add .db files from folder.

using (var z = ZipFile.Open(zipFile, ZipArchiveMode.Create)) {
	foreach (var f in filesystem.enumFiles(@"Q:\Test", "*.db"))
		z.CreateEntryFromFile(f.FullPath, f.Name);
}

/// Create .zip file from folder. Don't include subfolders named "protected" or "ico".

using (var z = ZipFile.Open(zipFile, ZipArchiveMode.Create)) {
	foreach (var f in filesystem.enumerate(@"Q:\Test\Folder", FEFlags.NeedRelativePaths, dirFilter: d => d.Name.Ends(true, @"\protected", @"\ico") > 0 ? 0 : 2)) {
		//print.it(f.Name);
		z.CreateEntryFromFile(f.FullPath, f.Name[1..].Replace('\\', '/'));
	}
}

/// Create .zip file and add files/folders specified in a list or array.

string list = @"
Q:\Test\sqlite.db
Q:\Test\Folder";
string[] a = list.Lines(noEmpty: true);
using (var z = ZipFile.Open(zipFile, ZipArchiveMode.Create)) {
	foreach (var f in a) {
		switch (filesystem.exists(f)) {
		case 1: //file
			z.CreateEntryFromFile(f, pathname.getName(f));
			break;
		case 2: //folder
			foreach (var k in filesystem.enumFiles(f, flags: FEFlags.AllDescendants | FEFlags.NeedRelativePaths))
				z.CreateEntryFromFile(k.FullPath, k.Name[1..].Replace('\\', '/'));
			break;
		default:
			print.warning("file not found: " + f);
			break;
		}
	}
}

/// List/find/extract zip file entries.

using (var z = ZipFile.OpenRead(zipFile)) {
	foreach (var e in z.Entries) {
		print.it(e.Name, e.FullName, e.Length, e.CompressedLength);
		if(e.Name.Eqi("m.txt")) {
			//var s = @"Q:\Test\Extracted\" + e.Name; //extract all to the same folder
			var s = @"Q:\Test\Extracted\" + e.FullName; //create subfolders if need
			filesystem.createDirectoryFor(s);
			e.ExtractToFile(s, overwrite: true);
		}
	}
}

/// If need other compression formats or more features, look for a zip library on the internet. More info in recipe ".NET and other libraries".

/// If need to compress/decompress data without creating files, use class <see cref="Convert2"/>. The <b>DeflateX</b> functions use zip compression; the <b>BrotliX</b> functions compress better.

byte[] b = new byte[10000]; for (int i = 0; i < b.Length; i++) b[i] = (byte)i;
byte[] b2 = Convert2.DeflateCompress(b);
print.it(b2.Length);
byte[] b3 = Convert2.DeflateDecompress(b2);
print.it(b3.Length);

/// If need to convert compressed data (or any binary data) from/to string, the best way is Base64 encoding.

string s1 = Convert.ToBase64String(b2);
b2 = Convert.FromBase64String(s1);
