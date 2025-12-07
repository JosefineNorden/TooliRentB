using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooLiRent.Services.DTOs.UsersDTOs
{
    public class UserListDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
