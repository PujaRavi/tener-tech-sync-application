Imports Microsoft.Owin
Imports Owin
Imports Microsoft.IdentityModel.Protocols.OpenIdConnect

Imports Microsoft.Owin.Security.Cookies

<Assembly: OwinStartup(GetType(NBHX_DR.Startup))>
Partial Public Class Startup
    Public Sub Configuration(app As IAppBuilder)
        ConfigureAuth(app)
    End Sub
End Class
