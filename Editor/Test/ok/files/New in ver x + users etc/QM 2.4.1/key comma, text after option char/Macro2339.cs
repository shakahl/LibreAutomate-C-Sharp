void RCompile::key_c()
{
	DeclareErrors("ERR_BADARG, ERR_HUNGWINDOW (on Abort)");

	//fid opt tt1 tok1 tt2 tok2 ... 0
		//opt: 1 +, 2 -, 128 as function
				//tt (token type):
					//1 keycodes
					//2 text
					//3 parentheses. Followed by: 1 int (vk etc), 2 string, 3 double (delay)
					//4 keycode repeat (after a keycode or 3(int))
					//6 {, 7 } (within keycodes)
	LPSTR s, g[1]; int opt=0, cpo, i, pt, vk, et, cet;

	if(line[0]=='\'')
	{
		SETI(F_KEY);
		s=line+1;
	} 
	else
	{
		if(cp>lines[currentline]+1) opt|=128;
		s=line+3;
		if(CHAR(*s)) switch(*s++) { case '+': opt=1; break; case '-': opt=2; break; }
		if(CHAR(*s)) err(0, s, 0);
		while(*s && NOTCHAR(*s)) s++; //don't use Tok(... 2) because it skips comma if key ,...
	}
	Tok(s, g, 1, 1, 1); s=g[0];
