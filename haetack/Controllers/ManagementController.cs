using haetack.Model;
using haetack.Model.DTO;
using haetack.Views.Management;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace haetack.Controllers
{
    public class ManagementController : Controller
    {
        HetekeeEntities db = new HetekeeEntities();

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("User");
            }
            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            var passedData = (ResultViewModel)TempData["data"];
            var viewModel = new ResultViewModel();

            if (passedData != null)
            {
                viewModel.Error = passedData.Error;
            };

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var viewModel = new ResultViewModel();

            if (ModelState.IsValid)
            {
                    var user = db.Admins.FirstOrDefault(p => p.username.Equals(username));

                    if (user != null)
                    {
                        if (user.isdeprecated)
                        {
                            viewModel.Error = "삭제된 계정입니다. 관리자에게 문의하세요";
                            viewModel.Username = username;
                            TempData["data"] = viewModel;
                            return View();
                        }
                        else
                        {
                            if (user.password.Equals(password))
                            {
                                var ticket = new FormsAuthenticationTicket(1, "userId", DateTime.Now, DateTime.Now.AddYears(1), true, user.C_id.ToString());

                                var encTicket = FormsAuthentication.Encrypt(ticket);
                                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                                return RedirectToAction("User", "Management");
                            }
                            else
                            {
                                viewModel.Error = "비밀번호가 일치하지 않습니다!";
                                viewModel.Username = username;
                                TempData["data"] = viewModel;
                                return View();
                            }
                        }
                    }
                    else
                    {
                        viewModel.Error = "존재하지 않은 아이디 입니다!";
                        viewModel.Username = username;
                        TempData["data"] = viewModel;
                        return View(viewModel);
                    }

            }
            else
            {
                viewModel.Error = "아이디와 비밀번호를 제대로 입력해주세요!";
                viewModel.Username = username;
                TempData["data"] = viewModel;
                return View(viewModel);
            }
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }




        public ActionResult User()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        public ActionResult UserDetail(int id)
        {
            if (Request.IsAuthenticated)
            {
                var passedData = (ResultViewModel)TempData["data"];
                var user = (from a in db.Admins
                            where a.C_id == id
                            select new UserDTO
                            {
                                Id = a.C_id,
                                Name = a.name,
                                Username = a.username,
                                Created = a.created,
                                Updated = a.updated,
                                Role = a.role,
                                IsDeprecated = a.isdeprecated
                            }).FirstOrDefault();

                if (user != null)
                {
                    var viewModel = new UserViewModel
                    {
                        User = user,
                        Message = passedData != null ? passedData.Error: null
                    };

                    return View(viewModel);
                }

                return RedirectToAction("User");
            }
            return RedirectToAction("Login");
            
        }
        public ActionResult EditUser(int id)
        {
            if (Request.IsAuthenticated)
            {
                var viewModel = new UserViewModel
                {
                    User = new UserDTO
                    {
                        Id = id,
                    }
                };

                return View(viewModel);
            }
            return RedirectToAction("Login");
        }
        public ActionResult ChangeStateUser (int id)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    var user = db.Admins.FirstOrDefault(x => x.C_id == id);

                    if (user != null)
                    {
                        user.isdeprecated = !user.isdeprecated;

                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {

                }

                return RedirectToAction("UserDetail", new { id = id });
            }
            return RedirectToAction("Login");
           
        }
        public ActionResult ChangePassword (int id, string message)
        {
            if (Request.IsAuthenticated)
            {
                var user = (from a in db.Admins
                            where a.C_id == id
                            select new UserDTO
                            {
                                Id = a.C_id,
                            }).FirstOrDefault();

                if (user != null)
                {
                    var viewModel = new UserViewModel
                    {
                        User = user,
                        Message = message
                    };

                    return View(viewModel);
                }

                return RedirectToAction("UserDetail", new { id = id });
            }
            return RedirectToAction("Login");
        }
        public ActionResult ChangeUserPic(int id)
        {
            if (Request.IsAuthenticated)
            {
                var viewModel = new UserViewModel
                {
                    User = new UserDTO
                    {
                        Id = id
                    }
                };
                return View(viewModel);
            }
            return RedirectToAction("Login");
        }
        public ActionResult ChangeUserRole(int id)
        {
            if (Request.IsAuthenticated)
            {
                using (var db = new HetekeeEntities())
                {
                    var user = (from a in db.Admins
                                where a.C_id == id
                                select new UserDTO
                                {
                                    Id = a.C_id,
                                    Role = a.role,
                                    Name = a.name
                                }).FirstOrDefault();

                    if (user != null)
                    {
                        var viewModel = new UserViewModel
                        {
                            User = user
                        };
                        return View(viewModel);
                    }

                    return RedirectToAction("UserDetail", new { id = id });
                }
            }
            return RedirectToAction("Login");
        }
        public ActionResult CreateUser(string message)
        {
            if (Request.IsAuthenticated)
            {
                var viewModel = new UserViewModel
                {
                    Message = message
                };

                return View(viewModel);
            }
            return RedirectToAction("Login");
        }
        
        [HttpPost]
        public ActionResult ChangeUserPic(int id, HttpPostedFileBase pic)
        {
            var viewModel = new ResultViewModel();
            try
            {
                var user = db.Admins.FirstOrDefault(x => x.C_id == id);

                if (user != null)
                {
                    var memoryStream = new MemoryStream();
                    pic.InputStream.CopyTo(memoryStream);
                    user.pic = memoryStream.ToArray();

                    db.SaveChanges();

                    return RedirectToAction("UserDetail", new { id = id });
                }

                viewModel.Error = "오류가 발생했습니다.";
                TempData["data"] = viewModel;
                return RedirectToAction("UserDetail", new { id = id });
            }
            catch
            {
                viewModel.Error = "오류가 발생했습니다.";
                TempData["data"] = viewModel;
                return RedirectToAction("UserDetail", new { id = id });
            }
        }
        [HttpPost]
        public ActionResult CreateUser(string name, string role, string username, string password, string confirmPass, HttpPostedFileBase pic)
        {
            if (Request.IsAuthenticated)
            {
                if (password.Equals(confirmPass))
                {
                    try
                    {
                        var user = new Admin
                        {
                            name = name,
                            username = username,
                            password = password,
                            created = DateTime.Now,
                            updated = DateTime.Now,
                            role = role,
                            isdeprecated = false
                        };

                        var memoryStream = new MemoryStream();
                        pic.InputStream.CopyTo(memoryStream);
                        user.pic = memoryStream.ToArray();

                        db.Admins.Add(user);
                        db.SaveChanges();

                        return RedirectToAction("User");
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("CreateUser", new { message = "회원을 추가하는데 오류가 발생했습니다." });
                    }
                }
                else
                {
                    return RedirectToAction("CreateUser", new { message = "비밀번호가 일치하지 않습니다." });
                }
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult ChangePassword(int id, string oldPass, string newPass, string confirmPass)
        {
            if (Request.IsAuthenticated)
            {
                var user = db.Admins.FirstOrDefault(x => x.C_id == id);

                if (user != null)
                {
                    if (user.password.Equals(oldPass))
                    {
                        if (newPass.Equals(confirmPass))
                        {
                            try
                            {
                                user.password = newPass;

                                db.SaveChanges();

                                return RedirectToAction("UserDetail", new { id = id });
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("ChangePassword", new { id = id, message = "새로운 비밀번호가 일치하지 않습니다." });
                    }
                }

                return RedirectToAction("ChangePassword", new { id = id, message = "비밀번호 변경하는데 오류가 발생했습니다" });
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult ChangeUserRole(int id, string role)
        {
            if (Request.IsAuthenticated)
            {
                var user = db.Admins.FirstOrDefault(x => x.C_id == id);

                if (user != null)
                {
                    try
                    {
                        user.role = role;

                        db.SaveChanges();

                        return RedirectToAction("UserDetail", new { id = id });
                    }
                    catch (Exception e)
                    {
                    }
                }

                return RedirectToAction("ChangeUserRole", new { id = id });
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult EditUser(int id, string name, string role)
        {
            var user = db.Admins.FirstOrDefault(x => x.C_id == id);

            if (user != null)
            {
                try
                {
                    user.name = name;
                    user.role = role;

                    db.SaveChanges();

                    return RedirectToAction("UserDetail", new { id = id });
                }
                catch (Exception e)
                {
                }
            }

            return RedirectToAction("ChangeUserRole", new { id = id });
        }




        public ActionResult Notice()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        public ActionResult CreateNotice()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        public ActionResult DetailNotice(int id, string message = "")
        {
            if (Request.IsAuthenticated)
            {
                var notice = (from a in db.Notices
                              where a.C_noticeid == id
                              select new NoticeDTO
                              {
                                  Id = a.C_noticeid,
                                  Title = a.title,
                                  Content = a.content,
                                  SubContent = a.subcontent,
                                  Created = a.created,
                                  Updated = a.updated,
                                  Hit = a.hit != null ? a.hit.Value : 0,
                                  IsDeprecated = a.isdeprecated
                              }).FirstOrDefault();

                var viewModel = new NoticeViewModel
                {
                    Notice = notice,
                    Message = message
                };

                return View(viewModel);
            }
            return RedirectToAction("Login");
        }
        public ActionResult ChangeStateNotice(int id)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    var item = db.Notices.FirstOrDefault(x => x.C_noticeid == id);
                    item.isdeprecated = !item.isdeprecated;
                    db.SaveChanges();

                    if (item.isdeprecated.Value)
                    {
                        return RedirectToAction("DetailNotice", new { id = id, message = "숨겨져있는 글입니다." });
                    }
                    else
                    {
                        return RedirectToAction("DetailNotice", new { id = id, message = "다시 게시되었습니다." });
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("DetailNotice", new { id = id, message = "숨기는데 오류가 발생했습니다." });
                }
            }
            return RedirectToAction("Login");
        }


        public ActionResult Pressless()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        public ActionResult CreatePressless()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        public ActionResult DetailPressless(int id, string message = "")
        {
            if (Request.IsAuthenticated)
            {
                var pressless = (from a in db.Presslesses
                              where a.C_presslessid == id
                              select new PresslessDTO
                              {
                                  Id = a.C_presslessid,
                                  Title = a.title,
                                  Content = a.content,
                                  SubContent = a.subcontent,
                                  Created = a.created,
                                  Updated = a.updated,
                                  PresslessUrl = a.presslessurl,
                                  Hit = a.hit != null ? a.hit.Value : 0,
                                  IsDeprecated = a.isdeprecated
                              }).FirstOrDefault();

                var viewModel = new PresslessViewModel
                {
                    Pressless = pressless,
                    Message = message
                    
                };

                return View(viewModel);
            }
            return RedirectToAction("Login");
        }
        public ActionResult ChangeStatePressless(int id)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    var item = db.Presslesses.FirstOrDefault(x => x.C_presslessid == id);
                    item.isdeprecated = !item.isdeprecated;
                    db.SaveChanges();

                    if (item.isdeprecated.Value)
                    {
                        return RedirectToAction("DetailPressless", new { id = id, message = "숨겨져있는 글입니다." });
                    }
                    else
                    {
                        return RedirectToAction("DetailPressless", new { id = id, message = "다시 게시되었습니다." });
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("DetailPressless", new { id = id, message = "숨기는데 오류가 발생했습니다." });
                }
            }
            return RedirectToAction("Login");
        }


        /* Json */
        public JsonResult GetUsers(Dictionary<string, string> sorting, Dictionary<string, string> filter, int page = 1, int count = 10)
        {
            var perPage = count;

            IQueryable<Admin> items = db.Admins;

            if (sorting != null)
            {
                var st = sorting.First();
                switch (st.Key)
                {
                    case "Id":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.C_id) : items.OrderByDescending(x => x.C_id);
                        break;
                    case "Name":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.name) : items.OrderByDescending(x => x.name);
                        break;
                    case "Username":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.username) : items.OrderByDescending(x => x.username);
                        break;
                    case "Created":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.created) : items.OrderByDescending(x => x.created);
                        break;
                    case "IsDeprecated":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.isdeprecated) : items.OrderByDescending(x => x.isdeprecated);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                items =
                    from p in items
                    orderby p.C_id descending
                    select p;
            }

            IQueryable<UserDTO> resultItems = (
                from a in items
                select new UserDTO
                {
                    Id = a.C_id,
                    Name = a.name,
                    Username = a.username,
                    Created = a.created,
                    Updated = a.updated,
                    IsDeprecated = a.isdeprecated
                });

            if (perPage == -1)
            {
                var allItems = resultItems.ToList();
                return Json(new ItemsDTO<UserDTO>
                {
                    Items = allItems,
                    PerPage = perPage,
                    RowCount = allItems.Count
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var allItems = resultItems.ToList();
                return Json(new ItemsDTO<UserDTO>
                {
                    Items = resultItems.Skip((page - 1) * perPage).Take(perPage).ToList<UserDTO>(),
                    PerPage = perPage,
                    RowCount = resultItems.Count()
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetNotices(Dictionary<string, string> sorting, Dictionary<string, string> filter, int page = 1, int count = 10)
        {
            var perPage = count;

            IQueryable<Notice> items = db.Notices;

            if (sorting != null)
            {
                var st = sorting.First();
                switch (st.Key)
                {
                    case "Id":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.C_noticeid) : items.OrderByDescending(x => x.C_noticeid);
                        break;
                    case "Title":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.title) : items.OrderByDescending(x => x.title);
                        break;
                    case "Created":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.created) : items.OrderByDescending(x => x.created);
                        break;
                    case "Updated":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.updated) : items.OrderByDescending(x => x.updated);
                        break;
                    case "IsDeprecated":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.isdeprecated) : items.OrderByDescending(x => x.isdeprecated);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                items =
                    from p in items
                    orderby p.C_noticeid descending
                    select p;
            }

            IQueryable<NoticeDTO> resultItems = (
                from a in items
                select new NoticeDTO
                {
                    Id = a.C_noticeid,
                    Title = a.title,
                    Created = a.created,
                    Updated = a.updated,
                    Hit = a.hit != null ? a.hit.Value : 0,
                    IsDeprecated = a.isdeprecated
                });

            if (perPage == -1)
            {
                var allItems = resultItems.ToList();
                return Json(new ItemsDTO<NoticeDTO>
                {
                    Items = allItems,
                    PerPage = perPage,
                    RowCount = allItems.Count
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var allItems = resultItems.ToList();
                return Json(new ItemsDTO<NoticeDTO>
                {
                    Items = resultItems.Skip((page - 1) * perPage).Take(perPage).ToList<NoticeDTO>(),
                    PerPage = perPage,
                    RowCount = resultItems.Count()
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetPresslesses(Dictionary<string, string> sorting, Dictionary<string, string> filter, int page = 1, int count = 10)
        {
            var perPage = count;

            IQueryable<Pressless> items = db.Presslesses;

            if (sorting != null)
            {
                var st = sorting.First();
                switch (st.Key)
                {
                    case "Id":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.C_presslessid) : items.OrderByDescending(x => x.C_presslessid);
                        break;
                    case "Title":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.title) : items.OrderByDescending(x => x.title);
                        break;
                    case "Created":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.created) : items.OrderByDescending(x => x.created);
                        break;
                    case "Updated":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.updated) : items.OrderByDescending(x => x.updated);
                        break;
                    case "IsDeprecated":
                        items = st.Value.Equals("asc") ? items.OrderBy(x => x.isdeprecated) : items.OrderByDescending(x => x.isdeprecated);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                items =
                    from p in items
                    orderby p.C_presslessid descending
                    select p;
            }

            IQueryable<PresslessDTO> resultItems = (
                from a in items
                select new PresslessDTO
                {
                    Id = a.C_presslessid,
                    Title = a.title,
                    Created = a.created,
                    Updated = a.updated,
                    Hit = a.hit != null ? a.hit.Value : 0,
                    IsDeprecated = a.isdeprecated
                });

            if (perPage == -1)
            {
                var allItems = resultItems.ToList();
                return Json(new ItemsDTO<PresslessDTO>
                {
                    Items = allItems,
                    PerPage = perPage,
                    RowCount = allItems.Count
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var allItems = resultItems.ToList();
                return Json(new ItemsDTO<PresslessDTO>
                {
                    Items = resultItems.Skip((page - 1) * perPage).Take(perPage).ToList<PresslessDTO>(),
                    PerPage = perPage,
                    RowCount = resultItems.Count()
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult CreateNotice(string title, string subContent, string content, string mainPic)
        {
            try
            {
                var notice = new Notice
                {
                    title = title,
                    subcontent = subContent,
                    content = content,
                    created = DateTime.Now,
                    updated = DateTime.Now,
                    hit = 0,
                    isdeprecated = false,
                };

                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var userData = ticket.UserData;
                var userId = int.Parse(userData);

                notice.for_userid = userId;

                db.Notices.Add(notice);

                db.SaveChanges();

                byte[] imageBytes = Convert.FromBase64String(mainPic.Split(',')[1]);
                var noticeImg = new NoticeImage
                {
                    content = imageBytes
                };
                db.NoticeImages.Add(noticeImg);
                db.SaveChanges();

                notice.NoticeImages.Add(noticeImg);

                db.SaveChanges();


                return Json(notice.C_noticeid, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var test = e;
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CreatePressless(string title, string subContent, string content, string mainPic, string url)
        {
            try
            {
                var pressless = new Pressless
                {
                    title = title,
                    subcontent = subContent,
                    content = content,
                    created = DateTime.Now,
                    updated = DateTime.Now,
                    hit = 0,
                    presslessurl = url,
                    isdeprecated = false,
                };

                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var userData = ticket.UserData;
                var userId = int.Parse(userData);

                pressless.for_userid = userId;

                db.Presslesses.Add(pressless);

                db.SaveChanges();

                byte[] imageBytes = Convert.FromBase64String(mainPic.Split(',')[1]);
                var presslessImg = new PresslessImage
                {
                    content = imageBytes
                };
                db.PresslessImages.Add(presslessImg);
                db.SaveChanges();

                pressless.PresslessImages.Add(presslessImg);

                db.SaveChanges();


                return Json(pressless.C_presslessid, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var test = e;
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult CreateNoticeImage(List<string> pics) //List<HttpPostedFileBase> pics)
        {
            try
            {
                var noticeImgArr = new List<NoticeImage>();
                foreach (var each in pics)
                {
                    byte[] imageBytes = Convert.FromBase64String(each.Split(',')[1]);
                    var noticeImg = new NoticeImage
                    {
                        content = imageBytes
                    };
                    db.NoticeImages.Add(noticeImg);
                    noticeImgArr.Add(noticeImg);
                }
                db.SaveChanges();


                var rtv = noticeImgArr.Select(x => x.C_noticeimageid).ToList();
                return Json(rtv, JsonRequestBehavior.AllowGet);

                //var rtv = new List<string>();
                //foreach (var pic in pics)
                //{
                //    if (pic != null && pic.ContentLength > 0)
                //    {
                //        string fname = Path.GetFileName(pic.FileName);
                //        var typeArr = pic.ContentType.Split('/');

                //        pic.SaveAs(Server.MapPath(Path.Combine("/Lib/TEST", fname)));

                //        var fileFullName = "/Lib/TEST/" + fname + "." + typeArr[1];
                //        rtv.Add(fileFullName);
                //    }
                //}

                //return Json(rtv, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CreatePresslessImage(List<string> pics)
        {
            try
            {
                var presslessImgArr = new List<PresslessImage>();
                foreach (var each in pics)
                {
                    byte[] imageBytes = Convert.FromBase64String(each.Split(',')[1]);
                    var presslessImage = new PresslessImage
                    {
                        content = imageBytes
                    };
                    db.PresslessImages.Add(presslessImage);
                    presslessImgArr.Add(presslessImage);
                }
                db.SaveChanges();


                var rtv = presslessImgArr.Select(x => x.C_presslessimageid).ToList();
                return Json(rtv, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteNotice(int id)
        {
            try
            {
                var item = db.Notices.FirstOrDefault(x => x.C_noticeid == id);
                item.NoticeImages.Clear();
                db.SaveChanges();

                db.Notices.Remove(item);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeletePressless(int id)
        {
            try
            {
                var item = db.Presslesses.FirstOrDefault(x => x.C_presslessid == id);
                item.PresslessImages.Clear();
                db.SaveChanges();

                db.Presslesses.Remove(item);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public void btnUploadClick(int id, HttpPostedFileBase pic)
        {
            //check file was submitted
            if (pic != null && pic.ContentLength > 0)
            {
                string fname = Path.GetFileName(pic.FileName);
                var test = pic.ContentType;
                var typeArr = pic.ContentType.Split('/');

                pic.SaveAs(Server.MapPath(Path.Combine("/Lib/TEST", fname)));
            }


         
        }

        public void Test(string url)
        {
            byte[] imageBytes = Encoding.Unicode.GetBytes(url);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            ms.Write(imageBytes, 0, imageBytes.Length);
            Image img = System.Drawing.Image.FromStream(ms);

            img.Save("/Lib/TEST", ImageFormat.Jpeg);
        }

    }
}
