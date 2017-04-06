 SetCurDir "Q:\app\Catkeys\Catkeys"
 out RunConsole2("q:\downloads\nuget.exe spec")

 SetCurDir "Q:\app\Catkeys\Catkeys"
 out RunConsole2("q:\downloads\nuget.exe pack catkeys.csproj")

 SetCurDir "Q:\app\Catkeys\Catkeys"
 out RunConsole2("q:\downloads\nuget.exe push Didgeridoo.Cats.1.0.0.0.nupkg cc29bcd0-3ebe-4e42-8a8b-3f4bf99c7202 -Source https://www.nuget.org/api/v2/package")
  https://www.nuget.org/packages/Didgeridoo.Cats/

SetCurDir "Q:\Test"
out RunConsole2("q:\downloads\nuget.exe install Didgeridoo.Cats -noninteractive")
