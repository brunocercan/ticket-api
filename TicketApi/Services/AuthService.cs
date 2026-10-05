using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TicketAPI.Models.Auth;
using TicketAPI.Models.Users;
using TicketAPI.DataTransferObjects;
using TicketAPI.Interfaces;

namespace TicketAPI.Services
{
    public class AuthService(IUserRepository userRepository, JwtSettings jwtSettings)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly JwtSettings _jwtSettings = jwtSettings;

        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return GenerateJwtToken(user);
        }

        public async Task<ConsultaUsuarioResponse?> RegisterAsync(CadastraUsuarioRequest request)
        {
            // Check if email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return null;
            }

            var userDto = new UsersDto
            {
                Name = request.Nome,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                Role = request.Funcao,
                CreatedAt = DateTime.Now
            };

            await _userRepository.CreateUserAsync(userDto);

            return new ConsultaUsuarioResponse
            {
                IdUsuario = userDto.Id,
                Nome = userDto.Name,
                EmailUsuario = userDto.Email,
                Funcao = userDto.Role,
                DataCriacao = userDto.CreatedAt
            };
        }

        private string GenerateJwtToken(UsersDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}