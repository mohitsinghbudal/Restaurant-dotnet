//using HotelManagementSystem.Interfaces.DatabaseConnection;
//using HotelManagementSystem.Models.User;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace HotelManagementSystem.Helper.JWT
//{
//    public class JWT: IJWT
//    {
//        private readonly IDbConnectionFactory _dbConnectionFactory;
//        public JWT (IDbConnectionFactory dbConnectionFactory)
//        {
//            _dbConnectionFactory = dbConnectionFactory;
            

//        }
//        public string GenerateTokenAsync(UserModel user)
//        {
//            var dbconnection = _dbConnectionFactory.CreateConnection();

//            var claims = new[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.FirstName, user.FirstName),
//                new Claim(ClaimTypes.Email, user.Email),
//                new Claim("RoleId", user.RoleId.ToString())
//            };

//            var key = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(
//                    dbconnection["Jwt:Key"]!));

//            var credentials = new SigningCredentials(
//                key,
//                SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: dbconnection["Jwt:Issuer"],
//                audience: dbconnection["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(2),
//                signingCredentials: credentials);

//            return new JwtSecurityTokenHandler()
//                .WriteToken(token);
//        }




//    }
//}
