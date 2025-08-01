﻿Imports System.Security.Claims
Imports System.Threading.Tasks
Imports Microsoft.Owin.Extensions
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.OpenIdConnect
Imports Owin

Partial Public Class Startup
    Private Shared clientId As String = ConfigurationManager.AppSettings("ida:ClientId")
    Private Shared aadInstance As String = EnsureTrailingSlash(ConfigurationManager.AppSettings("ida:AADInstance"))
    Private Shared tenantId As String = ConfigurationManager.AppSettings("ida:TenantId")
    Private Shared postLogoutRedirectUri As String = ConfigurationManager.AppSettings("ida:PostLogoutRedirectUri")
    Private Shared authority As String = aadInstance & tenantId & "/v2.0"

    Public Sub ConfigureAuth(app As IAppBuilder)
        app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType)

        app.UseCookieAuthentication(New CookieAuthenticationOptions())

        app.UseOpenIdConnectAuthentication(New OpenIdConnectAuthenticationOptions() With {
            .ClientId = clientId,
            .Authority = authority,
            .PostLogoutRedirectUri = postLogoutRedirectUri,
            .Notifications = New OpenIdConnectAuthenticationNotifications() With {
             .SecurityTokenValidated = Function(context)
                                           Dim name As String = context.AuthenticationTicket.Identity.FindFirst("preferred_username").Value
                                           context.AuthenticationTicket.Identity.AddClaim(New Claim(ClaimTypes.Name, name, String.Empty))
                                           Return Task.FromResult(0)
                                       End Function,
              .AuthenticationFailed = Function(context)
                                          Return Task.FromResult(0)
                                      End Function
              }
        })
        ' This makes any middleware defined above this line run before the Authorization rule is applied in web.config
        app.UseStageMarker(PipelineStage.Authenticate)
    End Sub

    Private Shared Function EnsureTrailingSlash(ByRef value As String) As String
        If (IsNothing(value)) Then
            value = String.Empty
        End If

        If (Not value.EndsWith("/", StringComparison.Ordinal)) Then
            Return value & "/"
        End If

        Return value
    End Function
End Class