using haetack.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace haetack.Views.Management
{
    public class UserViewModel
    {
        public UserDTO User { get; set; }
        public string Message { get; set; }
    }

    public class ResultViewModel
    {
        public string Error { get; set; }
        public string Username { get; set; }
    }

    public class NoticeViewModel
    {
        public NoticeDTO Notice { get; set; }
        public string Message { get; set; }
    }

    public class PresslessViewModel
    {
        public PresslessDTO Pressless { get; set; }
        public string Message { get; set; }
    }
}