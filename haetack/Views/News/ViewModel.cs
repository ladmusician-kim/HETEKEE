using haetack.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace haetack.Views.News
{
    public class NoticeViewModel
    {
        public ItemsDTO<NoticeDTO> Notices { get; set; }
        public NoticeDTO Notice { get; set; }
    }
    public class PresslessViewModel
    {
        public ItemsDTO<PresslessDTO> Presslesses { get; set; }
        public PresslessDTO Pressless { get; set; }
    }
}