using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Innopixel.Kontrolpanel.API.Controllers;

[Route("auth")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AuthenticationController : ControllerBase
{
	private readonly UserManager<ApplicationUser> userManager;
	private readonly RoleManager<IdentityRole> roleManager;
	private readonly IConfiguration configuration;

	public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
	{
		this.userManager = userManager;
		this.roleManager = roleManager;
		this.configuration = configuration;
	}

	[AllowAnonymous]
	[HttpPost("register-admin")]
	public async Task<IActionResult> RegisterAdmin(RegisterDTO model)
	{
		ApplicationUser userExists = await userManager.FindByNameAsync(model.Username);

		if (userExists is not null)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Bruger med denne email findes allerede!" });
		}

		ApplicationUser user = new()
		{
			Email = model.Email,
			SecurityStamp = Guid.NewGuid().ToString(),
			UserName = model.Username,
			CreatedDate = DateTime.Now
		};

		IdentityResult result = await userManager.CreateAsync(user, model.Password);

		if (!result.Succeeded)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Fejl ved oprettelse af bruger!" });
		}

		if (!await roleManager.RoleExistsAsync("Admin"))
		{
			await roleManager.CreateAsync(new IdentityRole("Admin"));
		}

		if (!await roleManager.RoleExistsAsync("Bruger"))
		{
			await roleManager.CreateAsync(new IdentityRole("Bruger"));
		}

		if (await roleManager.RoleExistsAsync("Admin"))
		{
			await userManager.AddToRoleAsync(user, "Admin");
		}

		return Ok(new { Status = "Success", Message = "Admin bruger oprettet!" });
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterDTO model)
	{
		ApplicationUser userExists = await userManager.FindByEmailAsync(model.Email);

		if (userExists is not null)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Bruger med denne email findes allerede!" });
		}

		if (!await roleManager.RoleExistsAsync("Admin"))
		{
			await roleManager.CreateAsync(new IdentityRole("Admin"));
		}

		if (!await roleManager.RoleExistsAsync("Bruger"))
		{
			await roleManager.CreateAsync(new IdentityRole("Bruger"));
		}

		ApplicationUser user = new ApplicationUser
		{
			Email = model.Email,
			SecurityStamp = Guid.NewGuid().ToString(),
			UserName = model.Username,
			CreatedDate = DateTime.Now
		};

		IdentityResult result = await userManager.CreateAsync(user, model.Password);

		if (!result.Succeeded)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Fejl ved oprettelse af bruger!" });
		}

		if (await roleManager.RoleExistsAsync("Bruger"))
		{
			await userManager.AddToRoleAsync(user, "Bruger");
		}

		return Ok(new { Status = "Success", Message = "Bruger oprettet!" });
	}

	[AllowAnonymous]
	[HttpPost("login")]
	public async Task<IActionResult> Login(LoginDTO model)
	{
		ApplicationUser user = await userManager.FindByEmailAsync(model.Email);

		if (user is not null)
		{
			if (await userManager.CheckPasswordAsync(user, model.Password))
			{
				List<string> userRoles = (List<string>)await userManager.GetRolesAsync(user);

				List<Claim> authClaims = new()
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				};

				foreach (string userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				X509Certificate2 certificate = new(configuration["X509Cert:FilePath"], configuration["X509Cert:Password"]);
				X509SecurityKey securityKey = new(certificate);

				JwtSecurityToken token = new(
					claims: authClaims,
					signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature),
					expires: model.RememberMe ? DateTime.Now.AddDays(365) : DateTime.Now.AddDays(1)
					);

				return Ok(new JwtSecurityTokenHandler().WriteToken(token));
			}
			else
			{
				return Unauthorized("Adgangskoden var forkert.");
			}
		}

		return Unauthorized("Emailen findes ikke i systemet.");
	}
}