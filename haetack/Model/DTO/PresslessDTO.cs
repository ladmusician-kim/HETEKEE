using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace haetack.Model.DTO
{
    public class PresslessDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SubContent { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Hit { get; set; }
        public bool? IsDeprecated { get; set; }
        public string PresslessUrl { get; set; }

        public int For_UserId { get; set; }
        public UserDTO User { get; set; }
    }
}