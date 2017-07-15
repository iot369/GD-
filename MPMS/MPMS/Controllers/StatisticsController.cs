using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MPMS.App_Start;
using MPMS.Models;
using System.Text;
using System.Data;
/*统计中心*/
namespace MPMS.Controllers
{
    public class StatisticsController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }
        //快速消费统计
        public ActionResult FastConsume()
        {
            ViewBag.cardLevelSel = db.CardLevels.Select(c => new SelectListItem() { Text = c.CL_LevelName, Value = c.CL_ID.ToString() });
            return View();
        }
        //快速消费统计
        [HttpPost]
        public JsonResult GetStatisticInfo(int id)
        {
            StringBuilder sb = new StringBuilder(string.Format("select * from ConsumeOrders, MemCards where ConsumeOrders.MC_ID = MemCards.MC_ID AND ConsumeOrders.S_ID = {0}", (Session["UserInfo"] as Users).S_ID));
            if (!string.IsNullOrEmpty(Request.Form["CO_OrderType"]))
            {
                sb.Append("AND CO_OrderType = " + Request.Form["CO_OrderType"].Trim());
            }
            else
            {
                sb.Append(id == -1 ? "" : "AND CO_OrderType = " + id + "");
            }
            if (!string.IsNullOrEmpty(Request.Form["CO_OrderCode"]))
            {
                sb.AppendFormat(" AND CO_OrderCode like '%{0}%'", Request.Form["CO_OrderCode"].Trim()); 
            }
            if (!string.IsNullOrEmpty(Request.Form["MC_CardID"]))
            {
                string id1 = Request.Form["MC_CardID"].Trim();
                if (id1.ToString().Trim().Length == 11)//手机号
                    id1 = (from i1 in db.MemCards where i1.MC_Mobile == id1.Trim() select i1).First().MC_CardID.ToString().Trim();
                sb.AppendFormat(" AND MemCards.MC_CardID like '%{0}%'", id1); 
            }
            if (!string.IsNullOrEmpty(Request.Form["cID"]))
            {
                sb.AppendFormat(" AND CL_ID = {0}", Request.Form["cID"].Trim()); 
            }
            if (!string.IsNullOrEmpty(Request.Form["CO_DiscountMoney"]) && !string.IsNullOrEmpty(Request.Form["ysf"]))
            {
                sb.AppendFormat(" AND CO_DiscountMoney {1} '{0}'", Request.Form["CO_DiscountMoney"].Trim(), Request.Form["ysf"].Trim()); 
            }
            else if (!string.IsNullOrEmpty(Request.Form["CO_GavePoint"]) && !string.IsNullOrEmpty(Request.Form["ysf"]))
            {
                sb.AppendFormat(" AND CO_GavePoint {1} '{0}'", Request.Form["CO_GavePoint"].Trim(), Request.Form["ysf"].Trim());
            }
            else if (!string.IsNullOrEmpty(Request.Form["CO_Recash"]) && !string.IsNullOrEmpty(Request.Form["ysf"]))
            {
                sb.AppendFormat(" AND CO_Recash {1} '{0}'", Request.Form["CO_Recash"].Trim(), Request.Form["ysf"].Trim());
            }
            if (!string.IsNullOrEmpty(Request.Form["BTime"]) && !string.IsNullOrEmpty(Request.Form["ETime"]))
            {
                sb.AppendFormat(" AND CO_CreateTime >= '{0}' AND  CO_CreateTime <= '{1}'", Request.Form["BTime"].Trim(), Request.Form["ETime"].Trim()); 
            }
            else if(!string.IsNullOrEmpty(Request.Form["BTime"]))
            {
                sb.AppendFormat(" AND CO_CreateTime >= '{0}'", Request.Form["BTime"].Trim());
            }
            else if (!string.IsNullOrEmpty(Request.Form["ETime"]))
            {
                sb.AppendFormat(" AND CO_CreateTime <= '{0}'", Request.Form["ETime"].Trim());
            }
            DataTable dt = SqlHelper.GetDataTable(sb.ToString());
            List<object> ml = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                ConsumeOrders c = dr.ToModel<ConsumeOrders>();
                MemCards m = dr.ToModel<MemCards>();
                ml.Add(new { m = m, c = c , CI_Name = (from i in db.CategoryItems where i.C_Category == "CO_OrderType" && i.CI_ID == c.CO_OrderType select i).First().CI_Name });
            }
            int pageSize = int.Parse(Request.Form["rows"] == "NaN" ? "1" : Request.Form["rows"]);
            int pageIndex = int.Parse(Request.Form["page"] == "NaN" ? "1" : Request.Form["page"]);
            return Json(new { total = ml.Count, rows = ml.Skip(--pageIndex * pageSize).Take(pageSize).ToList() }, JsonRequestBehavior.AllowGet);
        }
        //会员消费情况统计 
        public ActionResult MemCardConsume()
        {
            ViewBag.cO_OrderType = db.CategoryItems.Where(c => c.C_Category == "CO_OrderType").Select(c => new SelectListItem() { Text = c.CI_Name, Value = c.CI_ID.ToString() });
            return View();
        }
        public JsonResult GetMemCardInfo()
        {
            var ml = (from i in db.MemCards where i.S_ID == (Session["UserInfo"] as Users).S_ID select i).ToList();
            int pageSize = int.Parse(Request.Form["rows"] == "NaN" ? "1" : Request.Form["rows"]);
            int pageIndex = int.Parse(Request.Form["page"] == "NaN" ? "1" : Request.Form["page"]);
            return Json(new { total = ml.Count, rows = ml.Skip(--pageIndex * pageSize).Take(pageSize) }, JsonRequestBehavior.AllowGet);
        }
        //减积分统计
        public ActionResult ReduceIntegral()
        {
            ViewBag.cardLevelSel = db.CardLevels.Select(c => new SelectListItem() { Text = c.CL_LevelName, Value = c.CL_ID.ToString() });
            return View();
        }
        //积分返现统计
        public ActionResult IntegralToMoney()
        {
            ViewBag.cardLevelSel = db.CardLevels.Select(c => new SelectListItem() { Text = c.CL_LevelName, Value = c.CL_ID.ToString() });
            return View();
        }
        //礼品兑换统计
        public ActionResult ExchangGiftsExchang()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetExchangGiftsExchangInfo()
        {
            StringBuilder sb = new StringBuilder("select * from ExchangLogs where S_ID = " + (Session["UserInfo"] as Users).S_ID);
            if (!string.IsNullOrEmpty(Request.Form["MC_CardID"]))
            {
                string id1 = Request.Form["MC_CardID"].Trim();
                if (id1.ToString().Trim().Length == 11)//手机号
                    id1 = (from i1 in db.MemCards where i1.MC_Mobile == id1.Trim() select i1).First().MC_CardID.ToString().Trim();
                sb.AppendFormat(" AND MC_CardID like '%{0}%'", id1);
            }
            if (!string.IsNullOrEmpty(Request.Form["BTime"]) && !string.IsNullOrEmpty(Request.Form["ETime"]))
            {
                sb.AppendFormat(" AND EL_CreateTime >= '{0}' AND  CO_CreateTime <= '{1}'", Request.Form["BTime"].Trim(), Request.Form["ETime"].Trim());
            }
            else if (!string.IsNullOrEmpty(Request.Form["BTime"]))
            {
                sb.AppendFormat(" AND EL_CreateTime >= '{0}'", Request.Form["BTime"].Trim());
            }
            else if (!string.IsNullOrEmpty(Request.Form["ETime"]))
            {
                sb.AppendFormat(" AND EL_CreateTime <= '{0}'", Request.Form["ETime"].Trim());
            }
            DataTable dt = SqlHelper.GetDataTable(sb.ToString());
            List<ExchangLogs> ml = new List<ExchangLogs>();
            foreach (DataRow dr in dt.Rows)
            {
                ExchangLogs e = dr.ToModel<ExchangLogs>();
                ml.Add(e);
            }
            int pageSize = int.Parse(Request.Form["rows"] == "NaN" ? "1" : Request.Form["rows"]);
            int pageIndex = int.Parse(Request.Form["page"] == "NaN" ? "1" : Request.Form["page"]);
            return Json(new { total = ml.Count, rows = ml.Skip(--pageIndex * pageSize).Take(pageSize).ToList() }, JsonRequestBehavior.AllowGet);
        }
    }
}