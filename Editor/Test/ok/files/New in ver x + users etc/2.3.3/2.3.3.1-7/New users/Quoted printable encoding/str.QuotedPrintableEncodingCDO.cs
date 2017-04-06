 /
function$ action [$charset] ;;action: 0 encode, 1 decode

 Encodes or decodes text of this variable using quoted-printable encoding.
 Returns self.
 Error if fails. For example, if charset does not exist.

 charset - character set of encoded data. Eg "windows-1252". Default: "utf-8".

 NOTES
 Slow, especially first time.
 CDO bug: incorrectly encodes some Unicode characters.

 EXAMPLE
 str s="aaa=bbb []ą"
 out s.QuotedPrintableEncoding(0)
 out s.QuotedPrintableEncoding(1)


if(!this.len) ret this
if(empty(charset)) charset="utf-8"

IDispatch m._create("CDO.Message") b(m.BodyPart) s
b.ContentTransferEncoding="quoted-printable"

sel action
	case 0
	s=b.GetDecodedContentStream
	s.charset=charset
	s.WriteText(this); s.Flush
	s=b.GetEncodedContentStream
	
	case 1
	s=b.GetEncodedContentStream
	s.charset="windows-1252" ;;encoded data is ascii
	s.WriteText(this); s.Flush
	s=b.GetDecodedContentStream
	s.charset=charset
	
	case else ret this

this=s.ReadText
if(action=0 and !StrCompare(charset "utf-8" 1)) this.get(this 9) ;;BOM

err+ end _error
ret this

 notes:
 Also tested with typelib, but it is slower, and some interfaces unavailable in older QM versions.
