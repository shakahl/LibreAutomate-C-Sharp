Use System.String.
Class QM.Util.String_ adds extension methods to System.String.
	Their names end with _.
	Adds replacements for the bad compare/search String methods, and more methods.
str s

QM2				QM3

len(s)			s.Length
empty(s)		string.IsNullOrEmpty(s)
val(s)			s.ToInt_(), s.ToUInt_(), s.ToLong_(), s.ToDouble_() ;;unlike Convert.X, support 0xHEX, don't support current culture, etc. Overload with 'out numberLength' supports strings that only begin with a number; or all overloads support, and add ToIntStrict_() etc or a flag/bool param.
 or				Convert.ToInt32(s), Convert.ToInt64(s), Convert.ToDouble(s), int.Parse(), int.TryParse() etc
numlines(s)		s.LineCount_()
find(s)			s.Find_(), s.FindLast_() ;;s.[Last]IndexOf() dangerous
findw(s)		s.FindWord_()
findt(s)		s.FindWordN_() ;;rarely used
findl(s)		s.FindLineN_() ;;quite rarely used
tok(s)			s.Split(), s.Split_(), s.SplitEx_()
findc(s)		s.IndexOf()
findcr(s)		s.LastIndexOf()
findcs(s)		s.IndexOfAny()
 findcn(s)		 s.IndexOfNot_() ;;NONE: rarely used; easy to use Regex instead
 findb(s)		 s.FindBinary_() ;;NONE: almost never used; IndexOf can replace it in most cases
findrx(s)		Regex.Match(s)
matchw(s)		s.Like_()

s.lcase			s.ToLower_()
 or				s.ToLowerInvariant(), s.ToLower()
s.ucase			s.ToUpper_()
 or				s.ToUpperInvariant(), s.ToUpper()
s.unicode		Encoding.UTF8.GetChars() and other Encoding class functions ;;System.Text
s.ansi			Encoding.UTF8.GetBytes() and other Encoding class functions
s.escape		s.Escape_(), s.Unescape_(), s.UrlEscape_(), s.UrlUnescape_()
s.encrypt		s.BlaBlaBla_ ;;or aString.BlaBlaBla because maybe need encrypt byte[] etc. Or add another easy-to-use encryption class.
s.decrypt		s.BlaBlaBla_ ;;or aString.BlaBlaBla because maybe need decrypt byte[] etc. Or add another easy-to-use encryption class.
s.trim			s.Trim() ;;also s.Trim_(string characters)
s.ltrim			s.TrimStart() ;;also s.TrimStart_(string characters)
s.rtrim			s.TrimEnd() ;;also s.TrimEnd_(string characters)
s.set			s.SetChar_(char c, int startIndex, int count) ;;all my used set are s.set(char fron nc), don't need other overloads. The 'set string' overload is useless, can use aReplaceAt instead. This also should be able to append char[s].
s.insert		s.Insert(), StringBuilder.Insert()
s.remove		s.Remove(), StringBuilder.Remove()
s.replace		s.ReplaceAt_(string s, int startIndex, [int count])
s.findreplace	s.Replace(), s.ReplaceEx_(), StringBuilder.Replace()
s.replacerx		Regex.Replace()
s.addline		s.AppendLine_(), StringBuilder.AppendLine() ;;need to test StringBuilder's. addline very often used, but maybe better to have it only in StringBuilder, need to investigate how it is used in my code.
s.get			s.Get_ or s.Substring()
s.geta			s.GetAppend_() ;;quite rarely used (look also in C++ code). Can be replaced with s.Append_(s2.Get_()).
s.left			s.GetLeft_() ;;or s.Substring(0 n)
s.right			s.GetRight_() ;;or s.Substring(s.Length-n, n) ;;Maybe don't need, because in macros used only 1 time; look usage in C++ code.
s.gett			s.GetWordN_
s.getl			s.GetLineN_
s.getpath		s=Path.GetDirectoryName() ;;System.IO
s.getfilename	s=Path.GetFileName(), s=Path.GetFileNameWithoutExtension()
s.from			string.Concat(), operator +, $"string"
s.fromn			StringBuilder.Append() ;;or aString.ConcatLength_(), but it is problematic, unless we add many overloads
s.format		s=string.Format(), $"string"
s.formata		StringBuilder.AppendFormat(), +=$"string"
==				==, s.Equals(), string.Equals()
~=				s.EqualsI_(), s.Equals(), string.Equals()
!=				!=, !s.Equals(), !string.Equals()
s.beg[i]		s.Starts[I]_() ;;s.StartsWith() dangerous
s.end[i]		s.Ends[I]_() ;;s.EndsWith() dangerous
 s.mid[i]		NONE: rarely used; use string.Compare()
s.all			s=new string()
s.fix			s.Remove(), s.Limit_()
s.getwinX		s=w.GetX()
s.setwintext	w.SetName()
s.getclip		s=Clip.GetText(), s=Clipboard.GetText() ;;System.Windows.Forms
s.setclip		Clip.SetText(s), Clipboard.SetText(s)
s.getsel		s=Clip.Copy() or s=Send.Copy() or s=Input.Copy()
s.setsel		Clip.Paste(s) or Send.Paste(s) or s=Input.Paste(s)
s.getfile		s=Files.GetFileText(), s=File.ReadAllText() ;;System.IO. Need own function to support spec folders.
s.setfile		Files.SetFileText(s), File.WriteAllText(s)
s.searchpath	s=Files.SearchPath() ;;investigated: .NET does not have a wrapper method etc for API SearchPath
s.expandpath	s.ExpandPath_(), s=Files.ExpandPath(s)
s.dospath		s=Files.GetShortPath(), s=Files.GetLongPath() ;;investigated: .NET does not have it
s.getmacro		s=Script.GetText()
s.setmacro		Script.SetText(s)
s.dllerror		s=String_.WindowsApiError() ;;see Marshal.GetLastWin32Error. Maybe better to add to another class.
s.timeformat	s=string.Format(), $"text {DateTime.Now}"
s.get|set|struct	[Serializable], IFormatter etc
 s.swap			NONE: rarely used; usually we use return (not a ref/out parameter); also, usually we use StringBuilder.ToString().
s.ConvertEncoding
s.DateInFilename
s.FromGUID
s.GetClipboardHTML
s.GetFilenameExt
s.Guid
s.LimitLen
s.LoadUnicodeFile
s.RandomString
s.ReplaceInvalidFilenameCharacters
s.SaveUnicodeFile
s.SqlBlob
s.SqlEscape
s.stem
s.UniqueFileName
s.wrap







 TODO: investigate usage of questionable functions in my C++ code too.
 TODO: see also str.UDF.
 TODO: maybe use a prefix to our library namespaces, classes etc. Eg aPath class, to not conflict with existing and future .NET classes, namespaces etc.

 OTHER
s.AppendChar_(char c, int count=1) ;;or use StringBuilder.Append()
s.ToLines() ;;returns array of lines. Ex: foreach(string s2 in s1.ToLines()
String_.FromLines() ;;or use String.Join

------------------

s.IndexOf("fff")
s.IndexOf_("fff")
s.xIndexOf("fff")
s.IndexOf_("fff")
s.IndexOfX("fff")



S.EndsWith("fff")


var o2=new ScriptOptions(o1);
var o2=o1.Copy();
