using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MPMS.Models;
namespace MPMS.Controllers
{
    public class UserController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        // GET: User
        public ActionResult UserLogin()
        {
            return View();
        }

        //用户登入
        [HttpPost]
        public ActionResult UserLogin(string U_LoginName, string U_Password)
        {
            //声明一个变量存储 根据用户名密码查询出的结果集
            var result = from i in db.Users
                         where i.U_LoginName == U_LoginName && i.U_Password == U_Password
                         select i;
            //判断 如果结果集的数量大于0 则表示该用户存在
            if (result.Count() > 0)
            {
                Session["UserInfo"] = result.First();//将登入成功的用户数据保存在Session中
                return RedirectToAction("Index", "Admin");//页面跳转至Admin控制器的Index视图
            }
            else
            {
                //弹出提示框提示错误信息，并刷新页面
                return Content("<script> alert('用户名或密码错误');window.location.href= window.location.href</script>");
            }

        }

        // GET: User/CreateShopInfo
        public ActionResult CreateManage()
        {
            return View();
        }

        // POST: User/CreateShopInfo
        [HttpPost]
        public ActionResult CreateManage(Users u)
        {
            try
            {
                // TODO: Add insert logic here
                //判断登入名是否已经存在
                var num = (from i in db.Users
                           where i.U_LoginName == Request.Form["U_LoginName"]
                           select i).Count();
                if (num< 1)
                {
                    //保存修改
                    u.S_ID = Convert.ToInt32(Request.Form["S_ID"]);
                    u.U_LoginName = Request.Form["U_LoginName"];
                    u.U_Password = "123";
                    u.U_Role = 1;
                    u.U_CanDelete = true;
                    db.Users.InsertOnSubmit(u);
                    db.SubmitChanges();
                    return Content("OK");
                }
                return Content("NO");
            }
            catch
            {
                return View();
            }
        }

        //修改个人资料
        // GET: User/EditUserInfo/5
        public ActionResult EditUserInfo(int id)
        {
            //根据ID查询用户数据
            var oneUserInfo = (from i in db.Users
                               where i.S_ID == id
                               select i).First();
            //将查询到的数据返回至VIEW呈现
            return View(oneUserInfo);
        }

        // POST: User/EditUserInfo/5
        [HttpPost]
        public ActionResult EditUserInfo()
        {
            try
            {
                int U_ID = Convert.ToInt32(Request.Form["U_ID"]);
                //声明一个变量 存储用户选择的用户信息
                var oneM = (from i in db.Users
                            where i.U_ID == U_ID
                            select i).First();
                //判断登入名是否已经存在
                var num = (from i in db.Users
                             where i.U_LoginName == Request.Form["U_LoginName"]
                             select i).Count();
                //如果当前获取的登入名不等于当前登入用户的登入名且数据库已存在该登入名数据则返回"NO"
                if (!oneM.U_LoginName.Equals(Request.Form["U_LoginName"].ToString())&&num >0)
                {
                    return Content("NO");
                }
                //将用户修改后的数据 赋值给对应的实体项
                oneM.U_LoginName = Request.Form["U_LoginName"];
                oneM.U_RealName = Request.Form["U_RealName"];
                oneM.U_Telephone = Request.Form["U_Telephone"];
                oneM.U_Sex = Request.Form["U_Sex"];
                //保存修改
                db.SubmitChanges();
                return Content("OK");
            }
            catch (Exception ex)//捕捉异常
            {
                throw ex;//弹出异常
            }
        }

        //修改密码
        public ActionResult EditUserPWD(int id)
        {
            //根据ID查询用户数据
            var oneUserInfo = (from i in db.Users
                               where i.S_ID == id
                               select i).First();
            //将查询到的数据返回至VIEW呈现
            return View(oneUserInfo);
        }

        // POST: User/EditUserInfo/5
        [HttpPost]
        public ActionResult EditUserPWD()
        {
            try
            {
                int U_ID = Convert.ToInt32(Request.Form["U_ID"]);
                //声明一个变量 存储用户选择的用户信息
                var oneM = (from i in db.Users
                            where i.U_ID == U_ID
                            select i).First();
                //将用户修改后的数据 赋值给对应的实体项
                oneM.U_Password = Request.Form["U_newPwd2"];
                //保存修改
                db.SubmitChanges();
                return Content("OK");
            }
            catch (Exception ex)//捕捉异常
            {
                throw ex;//弹出异常
            }
        }
        // GET: User/DeleteShopInfo/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/DeleteShopInfo/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("ShopManage");
            }
            catch
            {
                return View();
            }
        }
        //退出系统
        public ActionResult Exit()
        {
            Session.Clear();
            return RedirectToAction("UserLogin");
        }
    }
}
