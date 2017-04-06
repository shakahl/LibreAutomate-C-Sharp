 /
function$ action ;;action: 0 encode, 1 decode

 Encodes or decodes text of this variable using quoted-printable encoding.
 Returns self.

 NOTES
 When encoding, for Unicode characters always uses default ANSI codepage of current user, regarless of whether QM is in Unicode mode. Therefore cannot be used Unicode characters that cannot be converted to ANSI.

 EXAMPLE
 str s="aaa=bbb []ą"
 out s.QuotedPrintableEncoding(0)
 out s.QuotedPrintableEncoding(1)


if(!this.len) ret this
MailBee.Message m._create

sel action
	case 0
	m.BodyText=this ;;does not respond to charset etc. Always uses default codepage/charset of current user.
	_s=m.RawBody
	
	case 1
	m.RawBody=_s.from(m.RawBody this)
	m.BodyEncoding=3 ;;at first encode base64, because if 0 (no encoding), wraps lines
	_s=m.RawBody
	
	case else ret this

 remove headeds
int i=find(_s "[][]")
if i>=0
	i+4
	if(action=0) this.get(_s i)
	else this.decrypt(4 _s+i); if(_unicode) this.ConvertEncoding(0 _unicode)

err+ end _error
ret this

 tested: with CDO.Message 30 times slower
