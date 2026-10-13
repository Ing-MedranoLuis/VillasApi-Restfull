using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTOS;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoyalVilla_API.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthServices(ApplicationDbContext context, IMapper mapper, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.applicationUsers.AnyAsync(e => e.Email.ToLower() == email.ToLower());
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {

            var user = await _context.applicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower());
            if (user == null)
            {
                return null;
            }
            var verified =await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (!verified)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            LoginResponseDTO loginResponseDTO= new LoginResponseDTO
            {
                token = await GenerateToken(user),
                user = _mapper.Map<UserDTO>(user)
            };


            loginResponseDTO.user.Role = roles.FirstOrDefault()??"Customer";
            return loginResponseDTO;
        }

        public async Task<UserDTO> RegisterAsync(RegisterRequestDTO registerRequestDTO)
        {
            if (await IsEmailExistAsync(registerRequestDTO.Email))
            {
                throw new NotImplementedException();
            }

            ApplicationUser user = new()
            {
                Email = registerRequestDTO.Email,
                Name = registerRequestDTO.Name,
                UserName=registerRequestDTO.Email,
                NormalizedEmail = registerRequestDTO.Email.ToUpper(),
                EmailConfirmed = true



            };

                    
            
            var result = await _userManager.CreateAsync(user,registerRequestDTO.Password);
            if (!result.Succeeded)
            {
                throw new NotImplementedException();
            }

            var role = String.IsNullOrEmpty(registerRequestDTO.Role) ? "Customer" : registerRequestDTO.Role;

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(user, role);




            await _context.SaveChangesAsync();
            var DTO = _mapper.Map<UserDTO>(user);
            DTO.Role = role;
            return DTO;
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtTokenKey")["SecureKey"]);
            var roles = await _userManager.GetRolesAsync(user);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                     new Claim(ClaimTypes.Name,user.Name),
                      new Claim(ClaimTypes.Email,user.Email),
                       new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
