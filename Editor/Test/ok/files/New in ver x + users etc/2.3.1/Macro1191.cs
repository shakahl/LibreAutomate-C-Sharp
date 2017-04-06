str s="RmF4IOrh6SBBY3JvYmF0IFdyaXRlcnMgzO/t3OT57SDRwcU="
s.decrypt(4) ;;decode base64
 out s

 s.unicode(s 28597) ;;ISO-8859-7 -> UTF-16
 s.ansi ;;UTF-16 to QM code page (UTF-8 in Unicode mode)
s.ConvertEncoding(28597 _unicode)
out s
