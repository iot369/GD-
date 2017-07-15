using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MPMS.Models;
using MPMS.App_Start;
using System.Data;
using System.Text;

namespace MPMS.Controllers
{
    public class FastConsumeController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        // GET: FastConsume
        public ActionResult Index()
        {
            return View();
        }
        //主界面
        public ActionResult FastConsume()
        {
            return View();
        }
        //判断会员是否存在 存在则返回实体
        public ActionResult GetMenInfo()
        {
            //var re = ("/^1[34578][0-9]{9}$/");
            string MidOrTel =Request.QueryString["mort"].Trim();
            StringBuilder sb =new StringBuilder("select c.CL_Point,c.CL_LevelName,c.CL_Percent,m.* from MemCards m join CardLevels c on c.CL_ID=m.CL_ID where 1=1");

            sb.Append(string.Format(MidOrTel.ToString().Length == 11 ? " and MC_Mobile='{0}'" : " and MC_CardID='{0}'", Request.QueryString["mort"].Trim()));

            DataTable dt = SqlHelper.GetDataTable(sb.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                Session["CriD"] = dt;
                DataRow dr = dt.Rows[0];
                MemCards m = dr.ToModel<MemCards>();
                CardLevels c = dr.ToModel<CardLevels>();
                return Json(new { MemCards = m, CardLevels = c }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "不存在" }, JsonRequestBehavior.AllowGet);
         }
        //快速消费
        public void Fastmoney()
        {
            try
            {
                ConsumeOrders co = new ConsumeOrders();
                DataTable Cdt = Session["CriD"] as DataTable;
                Users Udt = Session["UserInfo"] as Users;
                DateTime de = DateTime.Now;
                string Strde = de.ToString("yyyyMMddHHmmss");
                co.S_ID = Udt.S_ID;
                co.U_ID = Udt.U_ID;
                co.CO_OrderCode = Strde;
                co.CO_OrderType = 5;
                co.MC_ID =int.Parse(Cdt.Rows[0]["MC_ID"].ToString());
                co.MC_CardID = Cdt.Rows[0]["MC_CardID"].ToString();
                co.CO_TotalMoney = decimal.Parse(Request.Form["nMoney"]);
                co.CO_DiscountMoney = decimal.Parse(Request.Form["factMoney"]);
                co.CO_GavePoint = int.Parse(Request.Form["autoPoint"]);
                co.CO_CreateTime = de;
                co.CO_Remark = Request.Form["Remark"];
                db.ConsumeOrders.InsertOnSubmit(co);
                MemCards m = (from i in db.MemCards where i.MC_CardID == Cdt.Rows[0]["MC_CardID"].ToString() select i).First();
                
                m.MC_Point = int.Parse(Cdt.Rows[0]["MC_Point"].ToString()) + int.Parse(Request.Form["autoPoint"]);
                m.MC_TotalMoney = (float.Parse(Cdt.Rows[0]["MC_TotalMoney"].ToString())) + (float.Parse(Request.Form["factMoney"]));
                m.MC_TotalCount= (int.Parse(Cdt.Rows[0]["MC_TotalCount"].ToString())) + 1;
                db.SubmitChanges();
                Response.Write("ok");
            }
            catch (Exception ex)
            {   
                Response.Write(ex);
            }
        }
        //会员减积分
        public ActionResult MemCardChangePoint()
        {
            return View();
        }
        [HttpPost]
        //会员减积分
        public void ChangePoint()
        {
            try
            {
                var result = from i in db.MemCards where i.MC_CardID == Request.Form["mort"].Trim() select i;
                if (Request.Form["mort"].Trim().Length == 11) result = from i in db.MemCards where i.MC_Mobile == Request.Form["mort"] select i;
                MemCards m = result.First();
                m.MC_Point -= int.Parse(Request.Form["changePoint"]);//减积分
                                                                     //添加记录
                ConsumeOrders co = new ConsumeOrders();
                Users Udt = Session["UserInfo"] as Users;
                string Strde = DateTime.Now.ToString("yyyyMMddHHmmss");
                co.S_ID = Udt.S_ID;
                co.U_ID = Udt.U_ID;
                co.CO_OrderCode = Strde;
                co.CO_OrderType = 3;
                co.MC_ID = m.MC_ID;//int.Parse(Cdt.Rows[0]["MC_ID"].ToString());
                co.MC_CardID = m.MC_CardID;//Cdt.Rows[0]["MC_CardID"].ToString();
                co.CO_TotalMoney = 0;// decimal.Parse(Request.Form["nMoney"]);
                co.CO_DiscountMoney = 0;
                co.CO_GavePoint = int.Parse(Request.Form["changePoint"]);
                co.CO_CreateTime = DateTime.Now;
                co.CO_Remark = Request.Form["Remark"].Trim();
                db.ConsumeOrders.InsertOnSubmit(co);
                db.SubmitChanges();
                Response.Write("ok");
            }
            catch (Exception ex)
            {
                Response.Write("错误消息：" + ex);
            }
        }
        public ActionResult ConsumeLogs()
        {
            return View();
        }

    }
}