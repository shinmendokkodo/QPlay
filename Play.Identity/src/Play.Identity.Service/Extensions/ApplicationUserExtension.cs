using System;
using Play.Identity.Service.Models.Dtos;
using Play.Identity.Service.Models.Entities;

namespace Play.Identity.Service.Extensions
{
    public static class ApplicationUserExtension
    {
        public static UserDto AsDto(this ApplicationUser user)
        {
            return new UserDto
            (
                user.Id,
                user.UserName,
                user.Email,
                user.Gil,
                user.CreatedOn
            );
        }
    }
}
