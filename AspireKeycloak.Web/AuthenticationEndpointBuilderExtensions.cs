using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace AspireKeycloak.Web
{
	internal static class AuthenticationEndpointBuilderExtensions
	{
		internal static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder builder)
		{
			var group = builder.MapGroup("authentication");

			group
				.MapGet("/login", () => TypedResults.Challenge(new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/" }))
				.AllowAnonymous();

			group.MapPost("/logout", () => TypedResults.SignOut(
				new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/" },
				[CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

			return group;
		}
	}
}