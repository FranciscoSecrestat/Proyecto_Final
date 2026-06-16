using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

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

        
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            
            if (request.Password != request.ConfirmPassword)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Las contraseñas no coinciden"
                };
            }

            
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "El email ya está registrado"
                };
            }

            
            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                Password = HashPassword(request.Password),
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

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

        
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email o contraseña incorrectos"
                };
            }

           
            if (!VerifyPassword(request.Password, user.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email o contraseña incorrectos"
                };
            }

            
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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Email == email));
        }

        
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public async Task<bool> DepositAsync(int userId, decimal amount)
        {
            if (amount <= 0) return false;
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Balance += amount;

            _context.Deposits.Add(new Deposit
            {
                UserId = userId,
                Amount = amount,
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }


        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }

        public async Task<List<Deposit>> GetDepositsAsync(int userId)
        {
            return await _context.Deposits
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Date)
                .ToListAsync();
        }


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
             
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
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
