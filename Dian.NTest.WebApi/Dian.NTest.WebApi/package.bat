nuget pack Dian.NTest.WebApi.csproj
ping /n 2 127.1 >nul
nuget setapikey e69a7d51-8f6f-495f-82ec-50d0db75cf42
ping /n 2 127.1 >nul
nuget push Dian.NTest.WebApi*.nupkg