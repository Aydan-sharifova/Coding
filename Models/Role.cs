using System;
namespace Coding.Models
{
    public class Role : Base
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}

