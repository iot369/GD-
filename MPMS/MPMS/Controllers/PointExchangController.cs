using MPMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MPMS.Controllers
{//积分兑换
    public class PointExchangController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        // 兑换记录
        public ActionResult ExchangLog()
        {
            return View();
        }
        // 获得兑换记录
        public JsonResult GetExchangLogInfo(string id)
        {
            if (id.ToString().Trim().Length == 11)//手机号
                id = (from i1 in db.MemCards where i1.MC_Mobile == id.Trim() select i1).First().MC_CardID.ToString().Trim();
            var result = from i in db.ExchangLogs where i.MC_CardID == id.Trim() select i;
            int pageSize = int.Parse(Request.Form["rows"] == "NaN" ? "1" : Request.Form["rows"]);//获取并验证页面显示多少数量的验证
            int pageIndex = int.Parse(Request.Form["page"]);//获取并验证页面当前第几页
            return Json(new { total = result.Count(), rows = result.Skip(--pageIndex * pageSize).Take(pageSize) });
        }

        //查询会员卡信息
        public void SearchByID()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["MC_CardID"]))
            {
                string cardID = Request.QueryString["MC_CardID"];
                if (Request.QueryString["MC_CardID"].Length == 11)
                    cardID = (from i in db.MemCards where i.MC_Mobile == cardID select i).First().MC_CardID;
                var result = from i in db.MemCards where i.MC_CardID == cardID select i;
                if (result.Count() > 0)
                {
                    CardLevels c = (from i in db.CardLevels where i.CL_ID == result.First().CL_ID select i).First();
                    if (result.First().MC_State == 1) Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { CardLevels = c, MemCards = result.First() }));
                    else Response.Write("{\"msg\":\"该卡以被挂失，无法转账 \"} ");
                }
                else Response.Write("{\"msg\":\"查无此人\"} ");
            }
            else Response.Write("{\"msg\":\"未填写卡号!\"} ");
        }
        public ActionResult GetExchangGifts()
        {
            Users u = Session["UserInfo"] as Users;
            int id = u.S_ID.Value;

            int pageSize = 5, pageInsex = 0;
            if (Request.Form["rows"] != null)//rows 系统参数，当前显示行数
            {
                pageSize = int.Parse(Request.Form["rows"]);
            }
            if (Request.Form["page"] != null)//page系统参数，当前的页数 
            {
                pageInsex = int.Parse(Request.Form["page"]);

            }
            int skip = (pageInsex - 1) * pageSize;//需要除去的参数

            var result = (from i in db.ExchangGifts
                          where i.S_ID == id
                          select i).Skip(skip).Take(pageSize).ToList();
            return Json(new { total = db.ExchangGifts.Count(), rows = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //添加礼品记录
        public ActionResult Index(string ExchangLogs)
        {
            ViewBag.err = -1;
            try
            {
                Users u = Session["UserInfo"] as Users;
                int id = u.S_ID.Value;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<ExchangLogs> list = jss.Deserialize<List<ExchangLogs>>(ExchangLogs);
                for (int i = 0; i < list.Count; i++)
                {
                    ExchangLogs el = new Models.ExchangLogs();
                    el.S_ID = id;
                    el.U_ID = u.U_ID;
                    el.MC_CardID = list[i].MC_CardID;
                    el.MC_ID = list[i].MC_ID;
                    el.MC_Name = list[i].MC_Name;
                    el.EG_GiftName = list[i].EG_GiftName;
                    el.EL_CreateTime = DateTime.Now;
                    el.EL_Point = list[i].EL_Point;
                    el.EG_ID = list[i].EG_ID;
                    el.EG_GiftCode = list[i].EG_GiftCode;
                    el.EL_Number = list[i].EL_Number;
                    ExchangGifts ruslt = (from n in db.ExchangGifts
                                          where n.EG_ID == list[i].EG_ID
                                          select n).First();
                    MemCards m = (from n in db.MemCards
                                  where n.MC_CardID == list[i].MC_CardID
                                  select n).First();
                    m.MC_Point = m.MC_Point - list[i].EL_Point;
                    ruslt.EG_ExchangNum = (ruslt.EG_ExchangNum + list[i].EL_Number.Value);
                    db.ExchangLogs.InsertOnSubmit(el);
                }

                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                ViewBag.err = "错误消息" + ex.Message;
            }
            return View();
        }
        //积分返现
        public ActionResult PointToMoney()
        {
            return View();
        }
        //积分返现
        [HttpPost]
        public void PointToMoney(int id)
        {
            try
            {
                int changePoint = int.Parse(Request.Form["ExchangPoint"]);
                var m = db.MemCards.Where(me => me.MC_ID == id).First();
                m.MC_Point -= changePoint;//改变积分
                //添加记录
                ConsumeOrders co = new ConsumeOrders();
                Users Udt = Session["UserInfo"] as Users;
                string Strde = DateTime.Now.ToString("yyyyMMddHHmmss");
                co.S_ID = Udt.S_ID;
                co.U_ID = Udt.U_ID;
                co.CO_OrderCode = Strde;
                co.CO_OrderType = 2;
                co.MC_ID = m.MC_ID;//int.Parse(Cdt.Rows[0]["MC_ID"].ToString());
                co.MC_CardID = m.MC_CardID;//Cdt.Rows[0]["MC_CardID"].ToString();
                co.CO_TotalMoney = 0;// decimal.Parse(Request.Form["nMoney"]);
                co.CO_DiscountMoney = 0;
                co.CO_GavePoint = changePoint;
                co.CO_CreateTime = DateTime.Now;
                co.CO_Recash = int.Parse(Request.Form["ExchangMoney"]);
                db.ConsumeOrders.InsertOnSubmit(co);
                db.SubmitChanges();
                Response.Write("返现成功！");
            }
            catch (Exception ex)
            {
                Response.Write("错误消息:" + ex.Message);
            }
        }
    }
}