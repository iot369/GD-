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
    public class ManageUserController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        // GET: ManageUser
        public ActionResult Index()
        {
            return View();
        }
        //多条件模糊查询所有用户信息
        public ActionResult GetUserInfo()
        {
            string LoginName = Request.Form["LoginName"];
            string txtRealName = Request.Form["RealName"];
            string txtTelephone = Request.Form["Telephone"];
            int sID = Convert.ToInt32((Session["UserInfo"] as Users).S_ID);//登陆店主的店铺编号
            int pageSize = 5, pageIndex = 0;
            if (Request.Form["rows"] != null)//rows：系统参数，意思为：每页显示多少条数据
            {
                pageSize = int.Parse(Request.Form["rows"]);
            }
            if (Request.Form["page"] != null)//page:系统参数，意思为：当前 第几页
            {
                pageIndex = int.Parse(Request.Form["page"]);
            }
            int skip = (pageIndex - 1) * pageSize;//需要去除胡数据
            //var result = (from i in DB.Shops select i).Skip(skip).Take(pageSize).ToList();
            StringBuilder sb = new StringBuilder("select * from Users where 1=1 ");
            if (!string.IsNullOrEmpty(LoginName))
            {
                sb.Append(string.Format("and U_LoginName like '%{0}%'", LoginName));
            }
            else if (!string.IsNullOrEmpty(txtRealName))
            {
                sb.Append(string.Format(" and U_RealName like '%{0}%'", txtRealName));
            }
            else if (!string.IsNullOrEmpty(txtTelephone))
            {
                sb.Append(string.Format(" and U_Telephone like '%{0}%'", txtTelephone));
            } 
                sb.Append(string.Format(" and S_ID = '{0}'", sID));//店铺ID,该店铺管理员只能管理该店铺店员
            DataTable dt = SqlHelper.GetDataTable(sb.ToString());//得到数据Table
            //dataTable 转List<>泛型集合
            if (dt!=null&&dt.Rows.Count>0)
            {
                List<Users> list = new List<Users>();
                foreach (DataRow dr in dt.Rows)
                {
                    Users s = dr.ToModel<Users>();
                    list.Add(s);
                }
                //分页
                var result = list.Skip(skip).Take(pageSize).ToList();
                //返回Json格式的数据
                return Json(new { total = list.Count(), rows = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        // GET: ManageUser/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ManageUser/Create
        //新增用户信息
        public ActionResult Create()
        {
   
            //查询出店员及业务员信息
            var userType = (from i in db.CategoryItems
                            where i.C_Category == "U_Role"&&i.CI_ID!=1
                            select new
                            {
                                aa=i.CI_ID,
                                bb = i.CI_Name
                            }).ToList();
            //绑定至下拉列表
            ViewBag.category = new SelectList(userType, "aa", "bb");
            return View();
        }

        // POST: ManageUser/Create GetTypeName((int)model.BookType_ID);
        [HttpPost]
        //添加数据
        public ActionResult Creates()
        {
            try
            {
                //获取数据
                int sID =Convert.ToInt32((Session["UserInfo"] as Users).S_ID);
                string  uLoginName = Request.Form["ULoginName"];
                string uPassword = Request.Form["UPassword"];
                string uRealName = Request.Form["URealName"];
                string uSex =Request.Form["USex"];
                string uTelephone =Request.Form["UTelephone"];
                int uRole = int.Parse(Request.Form["URole"]);
                bool uCanDelete =bool.Parse(Request.Form["UCanDelete"]);

                var result = (from i in db.Users
                              where i.U_LoginName == uLoginName
                              select i);
                if (result.Count()<1)
                {
                    Users u = new Users();
                    //将修改的数据赋值给实体
                    u.U_CanDelete = uCanDelete;
                    u.S_ID = sID;
                    u.U_LoginName = uLoginName;
                    u.U_Password = uPassword;
                    u.U_RealName = uRealName;
                    u.U_Sex = uSex;
                    u.U_Telephone = uTelephone;
                    u.U_Role = uRole;
                    //添加数据，并保存修改
                    db.Users.InsertOnSubmit(u);
                    db.SubmitChanges();
                    return Content("OK");
                }
                return Content("ON");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: ManageUser/Edit/5
        public ActionResult Edit(int id)
         //查询用户数据并 返回view (绑定数据字段)
        {
            var result = (from i in db.Users
                          where i.U_ID == id
                          select i).First();
            var userType = (from i in db.CategoryItems
                            where i.C_Category == "U_Role"
                            select new
                            {
                                aa = i.CI_ID,
                                bb = i.CI_Name
                            }).ToList();
            ViewBag.category = new SelectList(userType, "aa", "bb");


            return View(result);

            //return RedirectToAction("Index");
        }
        [HttpPost]
        //编辑 保存修改信息
        public ActionResult Edit()
        {
            try
            {
                //获取数据
                int uid =int.Parse( Request.Form["UID"]);
                int sid = int.Parse(Request.Form["SID"]);
                string uLoginName = Request.Form["ULoginName"];
                string uPassword = Request.Form["UPassword"];
                string uRealName = Request.Form["URealName"];
                string uSex = Request.Form["USex"];
                string uTelephone = Request.Form["UTelephone"];
                int uRole =int.Parse(Request.Form["URole"]);
                int uCanDelete = bool.Parse(Request.Form["UCanDelete"])?1:0;
                //验证等入名是否存在
                var resultloginName = from i in db.Users
                             where i.U_LoginName == uLoginName&&i.U_ID!= uid
                                      select i;
                if (resultloginName.Count()<1)
                {
                var result = (from i in db.Users
                              where i.U_ID == uid
                              select i).First();
                result.U_ID = uid;
                result.S_ID = sid;
                result.U_LoginName = uLoginName;
                result.U_Password = uPassword;
                result.U_RealName = uRealName;
                result.U_Sex = uSex;
                result.U_Telephone = uTelephone;
                result.U_Role = uRole;
                result.U_CanDelete =Convert.ToBoolean( uCanDelete);
                    //result.U_CanDelete =uCanDelete;
                    db.SubmitChanges();
                    return Content("OK");
                }
                return Content("NO");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // GET: ManageUser/Delete/5
        //删除
        public ActionResult Delete()
        {
            int uid = int.Parse(Request.QueryString["uid"]);
            var result = (from i in db.Users
                          where i.U_ID == uid
                          select i).First();
            if (result.U_CanDelete==false)
            {
                return Content("NO");
            }
            db.Users.DeleteOnSubmit(result);
            db.SubmitChanges();
            return Content("OK");
        }
    }
}
