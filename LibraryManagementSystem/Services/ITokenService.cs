using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
