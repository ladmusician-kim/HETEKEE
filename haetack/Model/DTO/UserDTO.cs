using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace haetack.Model.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsDeprecated { get; set; }
        public string Role { get; set; }
    }
}