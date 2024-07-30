//using Domain.Entities;
//using Domain.Entities.DataTransferObject;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace Domain.Service.Services.ServiceJwt
//{
//    public class JwtTokenServiceAPI
//    {
//        private const int ExpirationMinutes = 30;
//        private readonly ILogger<JwtTokenServiceAPI> _logger;
//        private readonly IConfiguration _configuration;

//        public JwtTokenServiceAPI(ILogger<JwtTokenServiceAPI> logger, 
//            IConfiguration configuration
//            )
//        {
//            _logger = logger;
//            _configuration = configuration;
//        }

//        public TokenDto CreateToken(User user)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException(nameof(user));
//            }

//            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
//            var token = CreateJwtToken(
//                CreateClaims(user),
//                CreateSigningCredentials(_configuration),
//                expiration
//            );
//            _logger.LogInformation("JWT Token created");

//            return new TokenDto
//            {
//                Token = new JwtSecurityTokenHandler().WriteToken(token)
//            };
//        }

//        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
//            new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"],
//                audience: _configuration["Jwt:Audience"],
//                claims: claims,
//                expires: expiration,
//                signingCredentials: credentials
//            );

//        private List<Claim> CreateClaims(User user)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Chave simétrica não encontrada nas configurações.")),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
//                new Claim(ClaimTypes.NameIdentifier, user.Id ?? throw new ArgumentNullException(nameof(user.Id))),
//                new Claim(ClaimTypes.Name, user.UserName ?? throw new ArgumentNullException(nameof(user.UserName))),
//                new Claim(ClaimTypes.Email, user.Email ?? throw new ArgumentNullException(nameof(user.Email)))
//            };

//            var role = user.Role == 0 ? "Admin" : "User";
//            claims.Add(new Claim(ClaimTypes.Role, role) ?? throw new ArgumentNullException(nameof(user.Role)));

//            return claims;
//        }

//        private SigningCredentials CreateSigningCredentials(IConfiguration configuration)
//        {
//            var key = configuration["Jwt:Key"];
//            if (string.IsNullOrEmpty(key))
//            {
//                throw new InvalidOperationException("Chave simétrica não encontrada nas configurações.");
//            }

//            return new SigningCredentials(
//                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
//                SecurityAlgorithms.HmacSha256
//            );
//        }
//    }
//}