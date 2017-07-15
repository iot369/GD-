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
    public class GiftsController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        // GET: Gifts
        public ActionResult Index()
        {
            return View();
        }
        //查询所有礼品兑换信息
        public ActionResult GetExchangGifts()
        {
            string EG_GiftCode = Request.Form["EG_GiftCode"];
            string EG_GiftName = Request.Form["EG_GiftName"];
            int pageSize = 5, pageIndex = 0;
            if (Request.Form["rows"] != null)//rows：系统参数，意思为：每页显示多少条数据
            {
                pageSize = int.Parse(Request.Form["rows"]);
            }
            if (Request.Form["page"] != null)//rows：系统参数，意思为：当前第几页
            {
                pageIndex = int.Parse(Request.Form["page"]);
            }
            int skip = (pageIndex - 1) * pageSize;//需要去除的页数

            StringBuilder sb = new StringBuilder("select * from ExchangGifts where 1=1");
            if (!string.IsNullOrEmpty(EG_GiftCode))
            {
                sb.Append(string.Format(" and EG_GiftCode like '%{0}%'", EG_GiftCode));
            }
            if (!string.IsNullOrEmpty(EG_GiftName))
            {
                sb.Append(string.Format(" and EG_GiftName like '%{0}%'", EG_GiftName));
            }
            DataTable dt= SqlHelper.GetDataTable(sb.ToString());//根据SQL语句查询对应的数据
            if (dt != null && dt.Rows.Count > 0)//表数据不为空
            {
                List<ExchangGifts> list = new List<ExchangGifts>();//声明店铺类型的泛型实体
                foreach (DataRow dr in dt.Rows)//遍历表中的所有行数据
                {
                    ExchangGifts s = dr.ToModel<ExchangGifts>();//遍历一行数据，就把数据转换成对应的实体
                    list.Add(s);//把实体加载泛型之中
                }
                var result = list.Skip(skip).Take(pageSize).ToList();//分页
                return Json(new { total = db.ExchangGifts.Count(), rows = result }, JsonRequestBehavior.AllowGet);//把泛型转换成json格式
            }
            else
            {
                return null;
            }
        }

        // GET: Gifts/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Gifts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Gifts/Create
        [HttpPost]
        
        public ActionResult Create(ExchangGifts g)
        {
            try
            {
                string EG_GiftCode = Request.Form["EG_GiftCode"];
                string EG_GiftName = Request.Form["EG_GiftName"];
                int EG_Point = int.Parse(Request.Form["EG_Point"]);
                int EG_Number = int.Parse(Request.Form["EG_Number"]);
                string EG_Remark = Request.Form["EG_Remark"];
                var result = (from i in db.ExchangGifts
                           where i.EG_GiftCode == EG_GiftCode
                              select i).Count();
                if (result<1)
                {
                    g.EG_GiftCode = EG_GiftCode;
                    g.EG_GiftName = EG_GiftName;
                    g.EG_Point = EG_Point;
                    g.EG_Number = EG_Number;
                    g.EG_Remark = EG_Remark;
                    g.S_ID = 1;
                    g.EG_Photo = "";
                    g.EG_ExchangNum = 0;
                    db.ExchangGifts.InsertOnSubmit(g);
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

        // GET: Gifts/Edit/5
        public ActionResult Edit(int id)
        {
            var result = from i in db.ExchangGifts
                         where i.EG_ID == id
                         select i;
            return View(result.First());
        }

        // POST: Gifts/Edit/5
        //修改礼品信息
        [HttpPost]
        public ActionResult Edit()
        {
            try
            {
                //获取数据
                int EG_ID = int.Parse(Request.Form["EG_ID"]);
                string EG_GiftCode = Request.Form["EG_GiftCode"];
                string EG_GiftName = Request.Form["EG_GiftName"];
                string EG_Photo = Request.Form["EG_Photo"];
                int EG_Point = int.Parse(Request.Form["EG_Point"]);
                int EG_Number = int.Parse(Request.Form["EG_Number"]);
                string EG_Remark = Request.Form["EG_Remark"];
                //根据主键EG_ID编号查询数据
                var result = from i in db.ExchangGifts
                              where i.EG_ID == EG_ID
                             select i;
                var num = from i in db.ExchangGifts
                             where i.EG_GiftCode == EG_GiftCode
                          select i;
                //验证编码是否已经存在
                if (num.Count() >0 && !result.First().EG_GiftCode.Equals(EG_GiftCode))
                {
                    return Content("NO");
                }
                result.First().EG_ID = EG_ID;
                result.First().EG_GiftCode = EG_GiftCode;
                result.First().EG_GiftName = EG_GiftName;
                result.First().EG_Photo = EG_Photo;
                result.First().EG_Point = EG_Point;
                result.First().EG_Number = EG_Number;
                result.First().EG_Remark = EG_Remark;
                db.SubmitChanges();
                return Content("OK"); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Gifts/Delete/5
        //删除礼品信息
        public ActionResult Delete()
        {
            int EG_ID = int.Parse(Request.QueryString["EG_ID"]);
            var result = (from i in db.ExchangGifts
                          where i.EG_ID == EG_ID
                          select i).First();
            db.ExchangGifts.DeleteOnSubmit(result);
            db.SubmitChanges();
            return Content("OK");
        }
    }
}
