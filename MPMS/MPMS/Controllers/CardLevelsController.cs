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
    public class CardLevelsController : Controller
    {
        // GET: CardLevels
        LINQDBDataContext db = new LINQDBDataContext();
        public ActionResult Index()
        {
            return View();
        }
        //获取会员等级
        public ActionResult GetCardLevels()
        {
            try
            {
                string name = string.Empty;    //储存空值
                if (Request.Form["name"] !=null)  //判断有没有传值
                {
                    name = Request.Form["name"];  //获取
                }
                int pageSize = 4, pageIndex = 0;      //设置一行显示几条数据
                if (Request.Form["rows"] != null)   //判断
                {
                    pageSize = int.Parse(Request.Form["rows"]);  
                }
                if (Request.Form["page"] != null)   //当前页码数
                {
                    pageIndex = int.Parse(Request.Form["page"]);
                }
                int skip = (pageIndex - 1) * pageSize;  //需要去掉的数据

                //查询数据
                StringBuilder sb = new StringBuilder("select * from CardLevels where 1=1");
                if (!string.IsNullOrEmpty(name))   //判断不为空时，则添加查询条件
                {
                    sb.Append(string.Format(" and  CL_LevelName like '%{0}%'", name));
                }
                //根据Sql语句查询相对应的数据
                DataTable dt = SqlHelper.GetDataTable(sb.ToString());  
                //判断表数据不为空
                if (dt != null && dt.Rows.Count > 0)
                {
                    //声明会员等级的泛型实体
                    List<CardLevels> list = new List<CardLevels>();
                    //遍历表的所有行数据
                    foreach (DataRow row in dt.Rows)
                    {
                        //遍历所有数据，将数据转换成相应的实体
                        CardLevels c = row.ToModel<CardLevels>();
                        //将实体添加到集合当中
                        list.Add(c);
                    }
                    //分页
                    var result = list.Skip(skip).Take(4).ToList();
                    //将实体转换成Json格式
                    return Json(new { total = list.Count(), rows = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        //添加会员等级
        [HttpPost]
        public ActionResult Create(CardLevels c)
        {
            try
            {
                // 获取
                string CL_LevelName = Request.Form["CL_LevelName"];
                string CL_NeedPoint = Request.Form["CL_NeedPoint"];
                float CL_Point = float.Parse(Request.Form["CL_Point"]);
                float CL_Percent = float.Parse(Request.Form["CL_Percent"]);
                //根据用户输入的ID查询数据库数据
                var num = (from i in db.CardLevels
                            where i.CL_LevelName == CL_LevelName
                            select i).Count();
                //验证会员等级名称是否已经存在
                if (num<1)
                {
                    c.CL_LevelName = CL_LevelName;
                    c.CL_NeedPoint = CL_NeedPoint;
                    c.CL_Point = CL_Point;
                    c.CL_Percent = CL_Percent;
                    //提交  
                    db.CardLevels.InsertOnSubmit(c);
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


        // GET: CardLevels/Edit/5
        //编辑  查询会员信息并返回view
        public ActionResult Edit(int id)
        {
            //根据ID查询显示数据
            var result = (from i in db.CardLevels
                          where i.CL_ID == id
                          select i).First();
            return View(result);
        }

        // POST: CardLevels/Edit/5
        //编辑 保存修改的会员信息
        [HttpPost]
        public ActionResult Edit()
        {
            try
            {
                //获取
                int id = int.Parse(Request.Form["CL_ID"]);
                string LevelName = Request.Form["CL_LevelName"];
                string NeedPoint = Request.Form["CL_NeedPoint"];
                double Point = Convert.ToDouble(Request.Form["CL_Point"]);
                double Percent = Convert.ToDouble(Request.Form["CL_Percent"]);

                var result = (from i in db.CardLevels
                              where i.CL_ID == id
                              select i
                              ).First();
                //根据用户输入的LevelName查询数据库数据
                var num = (from i in db.CardLevels
                           where i.CL_LevelName == LevelName
                           select i).Count();
                //验证会员等级名称是否已经存在
                if (num < 0 || LevelName.Equals(result.CL_LevelName))
                {
                    //修改
                    result.CL_LevelName = LevelName;
                    result.CL_NeedPoint = NeedPoint;
                    result.CL_Point = Point;
                    result.CL_Percent = Percent;

                    //提交
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
        public ActionResult Delete(int id)
        {
            return View();
        }
        //删除会员信息
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                //获取id
                int id = int.Parse(Request.Form["CL_ID"]);
                //根据此会员等级ID查询是否含有会员信息
                var ishaveMemCardsInfo = (from i in db.MemCards
                                          where i.CL_ID == id
                                          select i).Count();
                if (ishaveMemCardsInfo < 1)
                {
                    //删除
                    var result = (from i in db.CardLevels
                                  where i.CL_ID == id
                                  select i).First();
                    db.CardLevels.DeleteOnSubmit(result);
                    //提交
                    db.SubmitChanges();
                    //返回
                    return Content("OK");
                }
                return Content("NO");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}