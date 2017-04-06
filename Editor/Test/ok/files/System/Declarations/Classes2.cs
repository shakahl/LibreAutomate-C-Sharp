 These classes are declared on demand, because we don't want to load type libraries at startup.

#ret

class Htm MSHTML.IHTMLElement'el
 ;;HTML element functions for IE. To insert code, use dialog 'Find HTML element'.
type ___EMAILACCOUNT ~accountname
	~smtp_server smtp_port ~smtp_user ~smtp_password !smtp_auth !smtp_secure smtp_timeout ~smtp_email ~smtp_displayname ~smtp_replyto
	~pop_server pop_port ~pop_user ~pop_password !pop_auth !pop_secure pop_timeout
type ___SMCOLL MailBee.Message'm @flags ~delfile
class SmtpMail -MailBee.SMTP'm_smtp -ARRAY(___SMCOLL)m_a -~m_folder
 ;;Sends email messages. To insert code, use dialog 'Send email message'.
class Pop3Mail MailBee.POP3'p -~m_folder
 ;;Receives email messages. To insert code, use dialog 'Receive email messages'.
class ExcelSheet Excel.Worksheet'ws : ExcelRange ExcelRow ExcelColumn FE_ExcelSheet_Row __ExcelState
#err __err_Silent
class __ExcelState -Excel.Application'm_a -!m_alertsOff -!m_updatingOff
#err __err_Silent
type EXCELFORMAT
	~style
	~numberFormat
	~fontName __fontStyle []{!fontBold !fontItalic !fontUnderline !fontNormal} fontSize fontColor
	cellColor
	alignHor alignVert !textWrap !textMerge !textNormal indentLevel
	borders borderThickness borderStyle borderColor
class Database ADO.Connection'conn : "?str format SqlEscape"
class HtmlDoc MSHTML.IHTMLDocument2'd MSHTML.IHTMLDocument3'd3 -m_flags -m_hax
 ;;Parses HTML.
