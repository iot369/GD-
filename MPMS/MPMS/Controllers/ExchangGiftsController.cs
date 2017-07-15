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
    public class ExchangGiftsController : Controller
    {
        LINQDBDataContext db = new LINQDBDataContext();
        // GET: ExchangGifts
        public ActionResult Index()
        {
            return View();
        }

        //查询所有礼品信息
        public ActionResult GetExchangGiftsInfo()
        {
            string GiftCode = string.Empty;
            string GiftName = string.Empty;
            if (Request.Form["giftCode"] != null)
            {
                GiftCode = Request.Form["giftCode"].Trim();
            }
            if (Request.Form["giftName"] != null)
            {
                GiftName = Request.Form["giftName"].Trim();
            }
            int pageSize = 3, pageIndex = 0;
            if (Request.Form["rows"] != null)
            {
                pageSize = int.Parse(Request.Form["rows"]);
            }
            if (Request.Form["page"] != null)
            {
                pageIndex = int.Parse(Request.Form["page"]);
            }
            int skip = (pageIndex - 1) * pageSize;
            //var result = (from i in db.ExchangGifts select i).Skip(skip).Take(pageSize).ToList();
            StringBuilder sb = new StringBuilder("select * from ExchangGifts where  S_ID =" + (Session["UserInfo"] as Users).S_ID + " ");
            if (!string.IsNullOrEmpty(GiftCode))
            {
                sb.Append(string.Format(" and EG_GiftCode like '%{0}%'", GiftCode));
            }
            if (!string.IsNullOrEmpty(GiftName))
            {
                sb.Append(string.Format(" and EG_GiftName like '%{0}%'", GiftName));
            }
            DataTable dt = SqlHelper.GetDataTable(sb.ToString());
            List<ExchangGifts> list = new List<ExchangGifts>();
            foreach (DataRow dr in dt.Rows)
            {
                ExchangGifts s = dr.ToModel<ExchangGifts>();
                list.Add(s);
            }
            var result = list.Skip(skip).Take(pageSize).ToList();
            return Json(new { total = db.ExchangGifts.Count(), rows = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: ExchangGifts/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExchangGifts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExchangGifts/Create
        [HttpPost]
        public ActionResult Create(ExchangGifts e)
        {
            try
            {
                // TODO: Add insert logic here
                return View();
            }
            catch
            {
                return View();
            }
        }

        public void IsNullCodeGifes()
        {
            string code = Request.Form["code"];
            if (Request.Form["id"] != null)
            {
                int id = int.Parse(Request.Form["id"]);
                var isnullCode = from i in db.ExchangGifts
                                 where i.EG_GiftCode == code && i.EG_ID != id
                                 select i;
                if (isnullCode.Count() > 0)
                {
                    Response.Write("err");
                }
            }
            else
            {
                var isnullCode = from i in db.ExchangGifts
                                 where i.EG_GiftCode == code
                                 select i;
                if (isnullCode.Count() > 0)
                {
                    Response.Write("err");
                }
            }
            
        }
        //添加
        public ActionResult CreateEX()
        {
            ExchangGifts es=new ExchangGifts();
            es.S_ID = (Session["UserInfo"] as Users).S_ID;
            es.EG_ExchangNum = 0;
            es.EG_GiftCode = Request.Form["eG_GiftCode"];
            es.EG_GiftName = Request.Form["eG_GiftName"];
            es.EG_Photo = Request.Form["eG_Photo"];
            es.EG_Point = int.Parse(Request.Form["eG_Point"]);
            es.EG_Number = int.Parse(Request.Form["eG_Number"]);
            es.EG_Remark = Request.Form["eG_Remark"];
            db.ExchangGifts.InsertOnSubmit(es);
            db.SubmitChanges();
            return Content("ok");
        }

        // GET: ExchangGifts/Edit/5
        public ActionResult Edit(int id)
        {
            var result = from i in db.ExchangGifts
                         where i.EG_ID == id
                         select i;
            return View(result.First());
        }

        // POST: ExchangGifts/Edit/5
        //更新礼品兑换信息
        [HttpPost]
        public ActionResult Edit()
        {
            try
            {
                // TODO: Add update logic here
                ExchangGifts es = (from i in db.ExchangGifts
                                   where i.EG_ID == int.Parse(Request.Form["EG_ID"])
                                   select i).First();
                es.EG_GiftCode = Request.Form["eG_GiftCode"];
                es.EG_GiftName = Request.Form["eG_GiftName"];
                es.EG_Photo = Request.Form["eG_Photo"];
                es.EG_Point = int.Parse(Request.Form["eG_Point"]);
                es.EG_Number = int.Parse(Request.Form["eG_Number"]);
                es.EG_Remark = Request.Form["eG_Remark"];
                db.SubmitChanges();
                return Content("ok");
            }
            catch(Exception ex)
            {
                return View(ex);
            }
        }

        // GET: ExchangGifts/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExchangGifts/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        
        public void Del(int id)
        {
            try
            {
                var result = (from i in db.ExchangGifts
                              where i.EG_ID == id
                              select i).First();
                db.ExchangGifts.DeleteOnSubmit(result);
                db.SubmitChanges();
                Response.Write("ok");
            }
            catch (Exception ex)
            {
                Response.Write(ex);
            }
        }
    }
}
