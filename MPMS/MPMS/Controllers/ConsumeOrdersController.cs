using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MPMS.App_Start;
using MPMS.Models;

namespace MPMS.Controllers
{
    public class ConsumeOrdersController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        // GET: ConsumeOrders
        public ActionResult Index()
        {
            return View();
        }

        //消费历史记录
        public ActionResult GetInfoByNumOrders()
        {
            var cardNum = Request.Form["num"];
            var Mobile = "";
            var CardID = "";
            //分页
            int pageSize = 2, pageIndex = 0;
            if (Request.Form["rows"] != null)  //每页显示的几条数据
            {
                pageSize = int.Parse(Request.Form["rows"]);
            }
            if (Request.Form["page"].Trim() != null)   //当前页面是第几页
            {
                pageIndex = int.Parse(Request.Form["page"]);
            }
            int skip = (pageIndex - 1) * pageSize;  //需要去除的数据，即当前页面要显示的数据
            if(cardNum==null)
            {
                return null;
            }
            else
            {
                if (cardNum.Length == 11)
                {
                    var num = (from i in db.MemCards     //判断手机号是否存在
                               where i.MC_Mobile == cardNum
                               select i).Count();
                    if (num > 0)
                    {
                        Mobile = cardNum;
                    }
                    else
                    {
                        Mobile = "";
                        return null;
                    }

                }
                else
                {
                    var num = (from i in db.MemCards    //判断卡号是否存在
                               where i.MC_CardID == cardNum
                               select i).Count();
                    if (num > 0)
                    {
                        CardID = cardNum;
                    }
                    else
                    {
                        CardID = "";
                        return null;
                    }

                }
            }
            
            string strSql = string.Format(@"select a.CL_LevelName,a.CL_Percent,b.MC_Point,b.MC_ID,b.MC_CardID,b.MC_Name,b.MC_TotalMoney,c.* from CardLevels a,MemCards b,ConsumeOrders c
                                           where b.MC_CardID='{0}' or b.MC_Mobile='{1}'", CardID, Mobile);
            DataTable dt = SqlHelper.GetDataTable(strSql);
            if (dt != null && dt.Rows.Count > 0)   //表数据不为空
            {
                List<Object> list = new List<Object>();    //申明泛型集合
                foreach (DataRow dr in dt.Rows)   //遍历
                {
                    CardLevels cl = dr.ToModel<CardLevels>();
                    MemCards mc = dr.ToModel<MemCards>();
                    ConsumeOrders co = dr.ToModel<ConsumeOrders>();
                    list.Add(new { CardLevels = cl, MemCards = mc, ConsumeOrders = co });
                }
                var result = list.Skip(skip).Take(pageSize).ToList();
                return Json(new { total = list.Count(), rows = result }, JsonRequestBehavior.AllowGet);
            }
            return null;
            
            
        }

        //快速消费 根据卡号查询
        public ActionResult GetInfoByNum()
        {
            var cardNum = Request.QueryString["num"];
            var Mobile = "";
            var CardID = "";
            if (cardNum.Length == 11)
            {
                var num = (from i in db.MemCards     //判断手机号是否存在
                           where i.MC_Mobile == cardNum
                           select i).Count();
                if(num>0)
                {
                    Mobile = cardNum;
                }
                else
                {
                    Mobile = "";
                    return null;
                }
                
            }
            else
            {
                var num = (from i in db.MemCards    //判断卡号是否存在
                           where i.MC_CardID == cardNum
                           select i).Count();
                if (num > 0)
                {
                    CardID = cardNum;
                }
                else
                {
                    CardID = "";
                    return null;
                }
                
            }
            string strSql = string.Format(@"select a.CL_LevelName,a.CL_Percent,b.MC_Point,b.MC_ID,b.MC_CardID,b.MC_Name,b.MC_TotalMoney from CardLevels a,MemCards b
                                           where b.MC_CardID='{0}' or b.MC_Mobile='{1}'", CardID, Mobile);
            DataTable dt = SqlHelper.GetDataTable(strSql);
            DataRow dr = dt.Rows[0];
            CardLevels cl = dr.ToModel<CardLevels>();
            MemCards mc = dr.ToModel<MemCards>();
            ConsumeOrders co = dr.ToModel<ConsumeOrders>();
            return Json(new { CardLevels = cl, MemCards = mc,ConsumeOrders=co }, JsonRequestBehavior.AllowGet);
        }

        //添加消费订单  快速消费
        public ActionResult InsertConsume(ConsumeOrders c)
        {
            MPMS.Models.Users user = Session["UserInfo"] as MPMS.Models.Users;
            var U_ID = user.U_ID;
            var S_ID = user.S_ID;
            var CO_TotalMoney = Request.Form["CO_TotalMoney"];
            var CO_DiscountMoney = Request.Form["CO_DiscountMoney"];
            var CO_CreateTime = Request.Form["CO_CreateTime"];
            var CO_OrderType = 5;
            var MC_ID = Request.Form["MC_ID"];
            var MC_CardID = Request.Form["MC_CardID"];

            c.CO_TotalMoney = decimal.Parse(CO_TotalMoney);
            c.CO_DiscountMoney = decimal.Parse(CO_DiscountMoney);
            c.CO_CreateTime = DateTime.Parse(CO_CreateTime);
            c.CO_OrderType = byte.Parse(CO_OrderType.ToString());
            c.MC_ID = int.Parse(MC_ID);
            c.MC_CardID = MC_CardID;
            c.U_ID = int.Parse(U_ID.ToString());
            c.S_ID = int.Parse(S_ID.ToString());

            db.ConsumeOrders.InsertOnSubmit(c);
            db.SubmitChanges();
            return Content("OK");
        }

        //添加消费订单  兑/减积分
        public ActionResult InsertConsumeGavePoint(ConsumeOrders c)
        {
            MPMS.Models.Users user = Session["UserInfo"] as MPMS.Models.Users;
            var U_ID = user.U_ID;
            var S_ID = user.S_ID;
            var CO_CreateTime = Request.Form["CO_CreateTime"];
            var CO_OrderType = 3;
            var MC_ID = Request.Form["MC_ID"];
            var MC_CardID = Request.Form["MC_CardID"];
            var CO_GavePoint = Request.Form["CO_GavePoint"];  //兑减积分
            var CO_Remark = Request.Form["CO_Remark"];  //备注

            c.CO_CreateTime = DateTime.Parse(CO_CreateTime);
            c.CO_OrderType = byte.Parse(CO_OrderType.ToString());
            c.MC_ID = int.Parse(MC_ID);
            c.MC_CardID = MC_CardID;
            c.U_ID = int.Parse(U_ID.ToString());
            c.S_ID = int.Parse(S_ID.ToString());
            c.CO_GavePoint = int.Parse(CO_GavePoint);
            c.CO_Remark = CO_Remark;

            db.ConsumeOrders.InsertOnSubmit(c);
            db.SubmitChanges();
            return Content("OK");
        }
        

        //会员减积分
        public ActionResult MemGavePoint()
        {
            return View();
        }


        //消费历史记录
        public ActionResult ConsumeLogs()
        {
            return View();
        }
    }
}