using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants.Permissions;
using static OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthorizationController: ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IUserEmailStore<AppUser> _emailStore;


    public AuthorizationController(SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager, IUserStore<AppUser> userStore)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
    }

    [HttpPost("signup")]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> SignUp([FromForm] string grant_type, [FromForm] string username, [FromForm] string password)
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            return BadRequest();
        }
        
        var user = new AppUser();
        
        await _userStore.SetUserNameAsync(user, request.Username, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, request.Username, CancellationToken.None);
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.Password);
        var result = await _userManager.CreateAsync(user);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, code);

            await _signInManager.SignInAsync(user, isPersistent: false);
            return SignIn(await SetPrincipal(user, request), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        var properties = new AuthenticationProperties(new Dictionary<string, string?>
        {
            [Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
            [Properties.ErrorDescription] =
                "Unable to create new user"
        });
        
        return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    [HttpPost("login")]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> LogIn([FromForm] string grant_type, [FromForm] string username, [FromForm] string password)
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
            return BadRequest();
        
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [Properties.ErrorDescription] = "The username/password couple is invalid."
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [Properties.ErrorDescription] = "The username/password couple is invalid."
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        return SignIn(await SetPrincipal(user, request), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<ClaimsPrincipal> SetPrincipal(AppUser user, OpenIddictRequest request)
    {
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        principal.SetScopes(new[] { Scopes.Email, Scopes.Roles }.Intersect(request.GetScopes()));
        principal.SetScopes(OpenIddictConstants.Scopes.OfflineAccess);

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }

        return principal;
    }
    
    private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Name:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(Scopes.Email))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Role:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(Scopes.Roles))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;
            
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
    }
    
    private IUserEmailStore<AppUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        { 
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<AppUser>)_userStore;
    }
}