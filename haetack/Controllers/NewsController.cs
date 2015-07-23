using haetack.Model;
using haetack.Model.DTO;
using haetack.Views.News;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace haetack.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /News/
        public ActionResult Index()
        {
            if (Request.Browser.Type.ToUpper().Contains("IE") || Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER")) // replace with your check
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    return RedirectToAction("Index", "Prepare");
                }
            }

            return RedirectToAction("Notice");
        }
        public ActionResult Notice(int page = 1, int perPage = 6)
        {
            if (Request.Browser.Type.ToUpper().Contains("IE") || Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER")) // replace with your check
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    return RedirectToAction("Index", "Prepare");
                }
            }
            var notices = DealNotice(GetNotices(page, perPage));
            var viewModel = new NoticeViewModel
            {
                Notices = notices
            };

            viewModel.Notices.PerPage = perPage;

            return View(viewModel);
        }
        public ActionResult DetailNotice(int id)
        {
            using (var db = new HetekeeEntities())
            {
                var notice = (from a in db.Notices
                              where a.C_noticeid == id
                              select new NoticeDTO
                              {
                                  Title = a.title,
                                  Content = a.content,
                                  SubContent = a.subcontent,
                                  Created = a.created,
                                  Updated = a.updated,
                                  Hit = a.hit != null ? a.hit.Value : 0,
                                  IsDeprecated = a.isdeprecated,
                                  For_UserId = a.for_userid != null ? a.for_userid.Value : -1,
                                  User = new UserDTO
                                  {
                                      Name = a.Admin.name,
                                      Role = a.Admin.role
                                  }
                              }).FirstOrDefault();

                var viewModel = new NoticeViewModel
                {
                    Notice = notice
                };

                return View(viewModel);
            }
        }




        public ActionResult Pressless(int page = 1, int perPage = 6)
        {
            if (Request.Browser.Type.ToUpper().Contains("IE") || Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER")) // replace with your check
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    return RedirectToAction("Index", "Prepare");
                }
            }
            var pressless = DealNotice(GetPresslesses(page, perPage));
            var viewModel = new PresslessViewModel
            {
                Presslesses = pressless
            };

            viewModel.Presslesses.PerPage = perPage;

            return View(viewModel);
        }
        public ActionResult DetailPressless(int id)
        {
            using (var db = new HetekeeEntities())
            {
                var pressless = (from a in db.Presslesses
                              where a.C_presslessid == id
                              select new PresslessDTO
                              {
                                  Title = a.title,
                                  Content = a.content,
                                  SubContent = a.subcontent,
                                  Created = a.created,
                                  Updated = a.updated,
                                  Hit = a.hit != null ? a.hit.Value : 0,
                                  IsDeprecated = a.isdeprecated,
                                  For_UserId = a.for_userid != null ? a.for_userid.Value : -1,
                                  PresslessUrl = a.presslessurl,
                                  User = new UserDTO
                                  {
                                      Name = a.Admin.name,
                                      Role = a.Admin.role
                                  }
                              }).FirstOrDefault();

                var viewModel = new PresslessViewModel
                {
                    Pressless = pressless
                };

                return View(viewModel);
            }
        }


















        public ItemsDTO<NoticeDTO> GetNotices(int page = 1, int count = 10)
        {
            var perPage = count;

            using (var db = new HetekeeEntities())
            {
                IQueryable<Notice> items = db.Notices;

                items = from p in items
                        orderby p.C_noticeid descending
                        select p;

                IQueryable<NoticeDTO> resultItems = (
                    from a in items
                    where a.isdeprecated != null && !a.isdeprecated.Value
                    select new NoticeDTO
                    {
                        Id = a.C_noticeid,
                        Title = a.title,
                        SubContent = a.subcontent,
                        Created = a.created,
                        Updated = a.updated,
                        Hit = a.hit != null ? a.hit.Value : 0,
                        IsDeprecated = a.isdeprecated,
                        For_UserId = a.for_userid != null ? a.for_userid.Value : -1
                    });

                if (perPage == -1)
                {
                    var allItems = resultItems.ToList();
                    return new ItemsDTO<NoticeDTO>
                    {
                        Items = allItems,
                        PerPage = perPage,
                        RowCount = allItems.Count,
                        Page = page
                    };
                }
                else
                {
                    var allItems = resultItems.ToList();
                    return new ItemsDTO<NoticeDTO>
                    {
                        Items = resultItems.Skip((page - 1) * perPage).Take(perPage).ToList<NoticeDTO>(),
                        PerPage = perPage,
                        RowCount = resultItems.Count(),
                        Page = page
                    };
                }
            }
        }
        public ItemsDTO<NoticeDTO> DealNotice(ItemsDTO<NoticeDTO> items)
        {
            var restVal = items.Items.Count % 3;
            if (restVal != 0)
            {
                var count = 0;
                if (restVal == 1)
                {
                    count = 2;
                }
                else if (restVal == 2)
                {
                    count = 1;
                }

                for(var i = 0; i < count; i ++) 
                {
                    items.Items.Add(new NoticeDTO());
                }
            }

            return items;
        }
        public FileResult GetMainPicForNotice(int id)
        {
            using (var db = new HetekeeEntities())
            {
                var notice = db.Notices.FirstOrDefault(x => x.C_noticeid == id);

                if (notice != null)
                {
                    var mainPic = notice.NoticeImages.FirstOrDefault();
                    if (mainPic != null)
                    {
                        return File(mainPic.content, "image/png");
                    }
                }

                return File("/Lib/images/layout/default_image.png", "image/png");
            }
        }
        public FileResult GetNoticeImage(int id)
        {
            using (var db = new HetekeeEntities())
            {
                var noticeImg = db.NoticeImages.FirstOrDefault(x => x.C_noticeimageid == id);

                if (noticeImg != null)
                {
                    return File(noticeImg.content, "image/png");
                }

                return File("/Lib/images/layout/default_image.png", "image/png");
            }
        }



        public ItemsDTO<PresslessDTO> GetPresslesses(int page = 1, int count = 10)
        {
            var perPage = count;

            using (var db = new HetekeeEntities())
            {
                IQueryable<Pressless> items = db.Presslesses;

                items = from p in items
                        orderby p.C_presslessid descending
                        select p;

                IQueryable<PresslessDTO> resultItems = (
                    from a in items
                    where a.isdeprecated != null && !a.isdeprecated.Value
                    select new PresslessDTO
                    {
                        Id = a.C_presslessid,
                        Title = a.title,
                        SubContent = a.subcontent,
                        Created = a.created,
                        Updated = a.updated,
                        Hit = a.hit != null ? a.hit.Value : 0,
                        IsDeprecated = a.isdeprecated
                    });

                if (perPage == -1)
                {
                    var allItems = resultItems.ToList();
                    return new ItemsDTO<PresslessDTO>
                    {
                        Items = allItems,
                        PerPage = perPage,
                        RowCount = allItems.Count,
                        Page = page
                    };
                }
                else
                {
                    var allItems = resultItems.ToList();
                    return new ItemsDTO<PresslessDTO>
                    {
                        Items = resultItems.Skip((page - 1) * perPage).Take(perPage).ToList<PresslessDTO>(),
                        PerPage = perPage,
                        RowCount = resultItems.Count(),
                        Page = page
                    };
                }
            }
        }
        public ItemsDTO<PresslessDTO> DealNotice(ItemsDTO<PresslessDTO> items)
        {
            var restVal = items.Items.Count % 3;
            if (restVal != 0)
            {
                var count = 0;
                if (restVal == 1)
                {
                    count = 2;
                }
                else if (restVal == 2)
                {
                    count = 1;
                }

                for (var i = 0; i < count; i++)
                {
                    items.Items.Add(new PresslessDTO());
                }
            }

            return items;
        }
        public FileResult GetMainPicForPressless(int id)
        {
            using (var db = new HetekeeEntities())
            {
                var pressless = db.Presslesses.FirstOrDefault(x => x.C_presslessid == id);

                if (pressless != null)
                {
                    var mainPic = pressless.PresslessImages.FirstOrDefault();
                    if (mainPic != null)
                    {
                        return File(mainPic.content, "image/png");
                    }
                }

                return File("/Lib/images/layout/default_image.png", "image/png");
            }
        }
        public FileResult GetPresslessImage(int id)
        {
            using (var db = new HetekeeEntities())
            {
                var preslessImg = db.PresslessImages.FirstOrDefault(x => x.C_presslessimageid == id);

                if (preslessImg != null)
                {
                    return File(preslessImg.content, "image/png");
                }

                return File("/Lib/images/layout/default_image.png", "image/png");
            }
        }



        public FileResult GetUserPic(int id)
        {
            using (var db = new HetekeeEntities())
            {
                var user = db.Admins.FirstOrDefault(x => x.C_id == id && x.isdeprecated != true);

                if (user != null && user.pic != null)
                {
                    return File(user.pic, "image/png");
                }

                return File("/Lib/images/layout/default_image.png", "image/png");
            }
        }
    }
}
