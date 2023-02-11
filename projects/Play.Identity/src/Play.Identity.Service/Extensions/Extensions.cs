using Play.Identity.Service.Dtos;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service.Extensions
{
    public static class Extensions
    {
        public static UserDto AsDto(this ApplicationUser user)
        {
            return new UserDto
            (
                Id : user.Id,
                Email : user.Email,
                Username : user.UserName,
                CreatedDate : user.CreatedOn,
                Gil : user.Gil
            );
        }
    }
}
