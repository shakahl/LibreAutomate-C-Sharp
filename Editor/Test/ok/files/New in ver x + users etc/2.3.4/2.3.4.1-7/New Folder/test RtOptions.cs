out
RTOPTIONS x xx

 x.flags=0x8000
 RtOptions 1 &x

 x.spe_for_macros=25
 RtOptions 2 &x

 x.waitcpu_time=5000
 RtOptions 4 &x

 x.waitcpu_threshold=50
 RtOptions 8 &x

x.web_browser_class="HH Parent"
RtOptions 16 &x

RtOptions 0 &xx
out _s.getstruct(xx 1)


 web "www.quickmacros.com" 1
