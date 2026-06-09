using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Registrar nuevo usuario
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Validar que las contraseñas coincidan
            if (request.Password != request.ConfirmPassword)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Las contraseñas no coinciden"
                };
            }

            // Validar que el email no exista
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "El email ya está registrado"
                };
            }

            // Crear nuevo usuario
            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                Password = HashPassword(request.Password),
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generar token
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name
                },
                Token = token
            };
        }

        // Iniciar sesión
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // Buscar usuario por email
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email o contraseña incorrectos"
                };
            }

            // Verificar contraseña
            if (!VerifyPassword(request.Password, user.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email o contraseña incorrectos"
                };
            }

            // Generar token
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Sesión iniciada exitosamente",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name
                },
                Token = token
            };
        }

        // Obtener usuario por email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Email == email));
        }

        // Hashear contraseña
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Verificar contraseña
        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }

        // Generar JWT Token
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"] ?? "mi-clave-secreta-super-larga-de-mas-de-32-caracteres"));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "ProyectoFinal",
                audience: _configuration["Jwt:Audience"] ?? "ProyectoFinalUsers",
                claims: new[]
                {
                    new System.Security.Claims.Claim("sub", user.Id.ToString()),
                    new System.Security.Claims.Claim("email", user.Email),
                    new System.Security.Claims.Claim("name", user.Name)
                },
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
