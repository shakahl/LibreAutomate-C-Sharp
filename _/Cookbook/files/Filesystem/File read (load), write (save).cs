/// There are many ways to read and write files of various formats.
/// Class <see cref="File"/> reads/writes simple text or raw binary data.

string file = @"C:\Test\test.txt";

/// Read all text to a string.

string text = File.ReadAllText(file);
print.it(text);

/// Read if file exists.

string text2 = File.Exists(file) ? File.ReadAllText(file) : null;

/// Write all text from a string.

File.WriteAllText(file, text); //creates new file or overwrites existing file

/// Read text lines.

string[] lines = File.ReadAllLines(file);
foreach (var s in lines) {
	print.it(s);
}

/// Write text lines from List.

var a = new List<string>();
for (int i = 1; i <= 10; i++) a.Add("line " + i);
File.WriteAllLines(file, a);

/// Usually text in files is Unicode, encoded as UTF-8 or UTF-16. The <b>ReadX</b> functions can read both. The <b>WriteX</b> functions write UTF-8 by default. Let's write UTF-16:

File.WriteAllText(file, text, Encoding.Unicode);

/// If the file already exists, the <b>WriteX</b> functions overwrite (replace) it, else they create new file.
/// The <b>AppendX</b> functions append to existing file or create new file.

File.AppendAllText(file, "line\r\n"); //append a string

File.AppendAllLines(file, a); //append a list or array

/// Read/write binary (non-text) data.

byte[] b = File.ReadAllBytes(file);
File.WriteAllBytes(file, b);
