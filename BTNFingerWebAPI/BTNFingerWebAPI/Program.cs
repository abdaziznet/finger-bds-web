using GenHTTP.Engine;

using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.Practices;
using GenHTTP.Modules.Security;
using GenHTTP.Modules.Webservices;

using BTNFingerWebAPI.Services;

var api = Layout.Create()
                //.AddService<BookService>("books")
                //.AddService<FingerService>("bdsweblogin")
                .AddService<FingerInitService>("init")
                .AddService<FingerSendService>("send")
                .AddService<FingerReceiveService>("receive")
                .Add(CorsPolicy.Permissive());

return Host.Create()
           .Handler(api)
           .Defaults()
           .Console()
#if DEBUG
           .Development()
#endif
           .Run();