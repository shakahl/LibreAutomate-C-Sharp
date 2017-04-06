function [colorBack] [colorProgress] [colorText]

 Initializes and sets parameters of icon drawing.
 Call before or after AddIcon. Can call several times to change parameters.


if(colorProgress=colorBack) colorProgress=0x00c000
if(colorText=colorBack) colorText=0xff

if m_hback
	m_hback.Delete
	m_hprogress.Delete
else
	m_mb.Create(16 16)
	m_font.Create("Arial" 7)

m_hback=CreateSolidBrush(colorBack)
m_hprogress=CreateSolidBrush(colorProgress)
SetTextColor m_mb.dc colorText
SetBkMode m_mb.dc TRANSPARENT
