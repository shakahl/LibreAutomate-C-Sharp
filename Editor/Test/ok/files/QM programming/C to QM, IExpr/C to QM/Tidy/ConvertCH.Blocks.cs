 /CtoQM

 Called by Tidy, to join {} and () blocks to single line.
 Also removes templates and extern "C" {.

m_s.replacerx("\bextern\s*''C\+*''\s*\{?")
m_s.replacerx("\bnamespace\s+\w+\s*\{")

 rep() if(!m_s.replacerx("\bextern\s*''C\+*''\s*({(((?>[^{}]+)|(?1))*)})?" "$2")) break
 this is faster than below, but would fail if there are strings with {}

 int i j
 rep
	 i=findrx(m_s "\bextern\s*''C\+*''\s*{" i 8 j); if(i<0) break
	  out "i=%i" i
	 m_s.set(32 i j); i+j
	 j=i+Brac2(m_s+i '{' '}')
	  out "j=%i" j
	 if(m_s[j]) m_s[j]=32
 m_s.replacerx("\bextern *''C\+*''")

m_s.replacerx("\btemplate\b[^{]+({((?>[^{}]+)|(?1))*});?" "")

BracSL('{' '}')
BracSL('(' ')')
