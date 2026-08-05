using System.Collections.Generic;
using HotelManagementSystem.Helper.JWT;
using HotelManagementSystem.Models.User;

namespace HotelManagementSystem.Interfaces.JWTInterface
{
    public interface IJWT
    {
       string JwtToken(UserModel user, List<int> roleIds);
    }
}
