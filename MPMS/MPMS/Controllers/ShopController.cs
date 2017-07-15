using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MPMS.Models;
using System.Text;
using System.Data;
using MPMS.App_Start;

namespace MPMS.Controllers
{
    public class ShopController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        //店铺信息首页
        // GET: Shop
        public ActionResult ShopManage()
        {
            return View();
        }
        public ActionResult GetShopInfo()
        {
            string sName = string.Empty;   //设置数据为空
            string cName = string.Empty;
            string Address = string.Empty;

            if (Request.Form["sName"] !=null)  //获取数据
            {
                sName = Request.Form["sName"];
            }
            if (Request.Form["cName"] != null)
            {
                cName = Request.Form["cName"];
            }
            if (Request.Form["Address"] != null)
            {
                Address = Request.Form["Address"];
            }
            //分页
            int pageSize = 2, pageIndex = 0;
            if(Request.Form["rows"]!=null)  //每页显示的几条数据
            {
                pageSize = int.Parse(Request.Form["rows"]);
            } 
            if(Request.Form["page"]!=null)   //当前页面是第几页
            {
                pageIndex = int.Parse(Request.Form["page"]);
            }
            int skip = (pageIndex - 1) * pageSize;  //需要去除的数据，即当前页面要显示的数据

            StringBuilder sb = new StringBuilder("select * from Shops where 1=1");   //使用SQL语句查询数据
            if(!string.IsNullOrEmpty(sName))   //如果用户名不为空 就查询
            {
                sb.Append(string.Format(" and S_Name like '%{0}%'",sName));
            }
            if(!string.IsNullOrEmpty(cName))
            {
                sb.Append(string.Format(" and S_ContactName like '%{0}%'",cName));
            }
            if (!string.IsNullOrEmpty(Address))
            {
                sb.Append(string.Format(" and S_Address like '%{0}%'", Address));
            }

            DataTable dt = SqlHelper.GetDataTable(sb.ToString());   //根据SQL语句查询对应的数据
            if(dt!=null && dt.Rows.Count>0)   //表数据不为空
            {
                List<Shops> list = new List<Shops>();    //申明泛型集合
                foreach (DataRow dr in dt.Rows)   //遍历
                {
                    Shops s = dr.ToModel<Shops>();  //遍历一行数据就转换成实体
                    list.Add(s);
                }
                var result = list.Skip(skip).Take(pageSize).ToList();   //去掉前面的再显示pageSize条数据  进行分页
                return Json(new { total = list.Count(), rows = result }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        // GET: Shop/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Shop/CreateShopInfo
        public ActionResult CreateShopInfo()
        {
            //查询所有店铺类型
            var shopInfo = (from i in db.CategoryItems
                           where i.C_Category == "S_Category"
                           select new {
                               sID = i.CI_ID,
                               sName =i.CI_Name
                           }).ToList();
            //将数据绑定至下拉列表
            ViewBag.category = new SelectList(shopInfo, "sID", "sName");
            return View();
        }

        // POST: Shop/CreateShopInfo
        [HttpPost]
        public ActionResult CreateShopInfo(Shops s)
        {
            try
            {
                //获取数据
                s.S_Name = Request.Form["S_Name"];
                s.S_Category= int.Parse(Request.Form["S_Category"]);
                s.S_ContactName= Request.Form["S_ContactName"];
                s.S_ContactTel= Request.Form["S_ContactTel"];
                s.S_Address= Request.Form["S_Address"];
                s.S_Remark= Request.Form["S_Remark"];

                s.S_CreateTime = DateTime.Now;//加盟时间
                s.S_IsHasSetAdmin = false;//默认无管理员

                //添加数据
                db.Shops.InsertOnSubmit(s);
                //保存修改
                db.SubmitChanges();

                return Content("OK");
            }
            catch
            {
                return View();
            }
        }

        // GET: Shop/EditUserInfo/5 修改店铺 信息
        public ActionResult EditShopInfo(int id)
        {
            //查询出所选择的店铺信息 回传至页面
            var result = (from i in db.Shops
                          where i.S_ID == id
                          select i).First();

            //先查出所有的店铺类型
            var shopInfo = from i in db.CategoryItems
                           where i.C_Category == "S_Category"
                           select new
                           {
                               sId = i.CI_ID,
                               sName = i.CI_Name
                           };
            ViewBag.category = new SelectList(shopInfo, "sId", "sName");   //传递到页面,并帮定到下拉列表
            return View(result);
        }

        // POST: Shop/EditUserInfo/5 修改店铺 信息
        [HttpPost]
        public ActionResult EditShopInfo()
        {
            try
            {
                //获取数据
                int S_ID = int.Parse(Request.Form["S_ID"]);
                string S_Name = Request.Form["S_Name"];
                int S_Category = int.Parse(Request.Form["S_Category"]);
                string S_ContactName = Request.Form["S_ContactName"];
                string S_ContactTel = Request.Form["S_ContactTel"];
                string S_Address = Request.Form["S_Address"];
                string S_Remark = Request.Form["S_Remark"];

                var shops = (from i in db.Shops
                             where i.S_ID == S_ID
                             select i).First();
                //修改数据
                shops.S_Name = S_Name;
                shops.S_Category = S_Category;
                shops.S_ContactName = S_ContactName;
                shops.S_ContactTel = S_ContactTel;
                shops.S_Address = S_Address;
                shops.S_Remark = S_Remark;
                //保存修改
                db.SubmitChanges();
                return Content("OK");
            }
            catch
            {
                return View();
            }
        }

        // GET: Shop/Edit添加管理员   
        public ActionResult EditAdmin(int id)
        {
            var result = (from i in db.Shops
                          where i.S_ID == id
                          select i).First();
            return View(result);
        }

        // POST: Shop/Edit添加管理员
        [HttpPost]
        public ActionResult EditAdmin()
        {
            try
            {
                var sId = int.Parse(Request.Form["S_ID"]);
                var shop = (from i in db.Shops
                            where i.S_ID == sId
                            select i).First();
                shop.S_IsHasSetAdmin = true;
                db.SubmitChanges();
                return Content("OK");
            }
            catch
            {
                return View();
            }
        }

        // GET: Shop/DeleteShopInfo/5 
        public ActionResult DeleteShopInfo()
        {
            var id = int.Parse(Request.QueryString["sID"]);
            //删除管理员信息
            var userM = (from u in db.Users where u.S_ID == id && u.U_Role.Equals(1) select u).First();
            db.Users.DeleteOnSubmit(userM);
            db.SubmitChanges();
            //删除店铺信息
            var result = (from i in db.Shops
                          where i.S_ID == id
                          select i).First();
            db.Shops.DeleteOnSubmit(result);
            db.SubmitChanges();
            return Content("OK");
        }

    }
}
