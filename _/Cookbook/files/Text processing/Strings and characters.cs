/// A string is an array of letters, numbers, spaces and other characters enclosed in "".More

string s = "Abc, 12";
char c1 = s[2]; //c

/// Characters in a string can't be changed. String functions never change string content; they create new string.

//s[2] = 'd'; //error
string s2 = s.ReplaceAt(2, 1, "d"); //creates new string
print.it(s, s2);

/// Character arrays can have the same content as strings. Characters in them can be changed.

char[] a = s.ToCharArray();
a[2] = 'e';
s = new string(a); //assign new string to variable s; forget the old string
print.it(s);

/// Get character properties.

char c2 = 'A';
print.it(char.IsLetter(c2), char.IsUpper(c2), char.GetUnicodeCategory(c2), c2.IsAsciiDigit());

/// Get uppercase or lowercase version.

print.it(char.ToUpper(c1), char.ToLower(c2), s.Upper());

/// Enumerate characters in a string or array. Get character codes.

for (int i = 0; i < s.Length; i++) {
	var c3 = s[i];
	print.it(c3, (ushort)c3);
}

/// Strings and characters are Unicode UTF-16. A character is 2 bytes, more than 65000 possible values. More characters exists, but they are rarely used; they are encoded as 2 surrogate char.
