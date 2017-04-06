 Get value of environment variable temp:
  #compile "winapi"

 int s
 lpstr p
 s = GetEnvironmentVariable("PATH",&p,63)
 out s
 out p
 produces an "Illegal Operation" error and crashes QM

 int l = 1
 str p
 p.all(256)
 l = GetEnvironmentVariable("TEMP",p,256)
 p.fix(l)
 out p
 out l
 this finally gives me the result: have to initialize p
int l =1 ;;= GetEnvironmentVariable("TEMP",0,0)
str p
p.all(16)
out l
l = GetEnvironmentVariable("TEMP",p,16)
out len(p)
p.fix(l);; without this and without p.all or (p = spaces) p ends up being blank or something!
out p
out "ok"
 Create or change environment variable var1:
 _putenv("var1=value")

 Delete environment variable var2:
 _putenv("var2=")


 function str'envar lpstr&enstr
 int l = GetEnvironmentVariable(envar,0,0)
 l = GetEnvironmentVariable(envar,enstr,l)
 ret l
 BUG: produced "Exception in getenv"
  call from: 
 str path
 int x = getenvDOS("PATH",path)
 out path
