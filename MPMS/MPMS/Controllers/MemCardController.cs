using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MPMS.Models;
using System.Text;
using System.Data;
using MPMS.App_Start;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data.OleDb;

namespace MPMS.Controllers
{
    public class MemCardController : Controller
    {
        //支持的Excel版本格式
        private const string ConnString2003 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes'";
        private const string ConnString2007 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes'";

        LINQDBDataContext db = new LINQDBDataContext();
        // GET: MemCard
        public ActionResult Index()
        {
            if (Session["UserInfo"] == null) return RedirectToAction("UserLogin", "User");
            ViewBag.mC_StateSel = db.CategoryItems.Where(c => c.C_Category == "MC_State").Select(c => new SelectListItem() { Text = c.CI_Name, Value = c.CI_ID.ToString() });
            ViewBag.cardLevelSel = db.CardLevels.Select(c => new SelectListItem() { Text = c.CL_LevelName, Value = c.CL_ID.ToString() });
            return View();
        }
        //挂失/锁定
        public ActionResult ReportTheLossOfSthORLock(int id)
        {
            ViewBag.mC_StateSel = db.CategoryItems.Where(c => c.C_Category == "MC_State").Select(c => new SelectListItem() { Text = c.CI_Name, Value = c.CI_ID.ToString() });
            return View((from i in db.MemCards where i.MC_ID == id select i).First());
        }
        //挂失/锁定
        [HttpPost]
        public void ReportTheLossOfSthORLock()
        {
            try
            {
                (from i in db.MemCards where i.MC_CardID == Request.Form["MC_CardID"] select i).First().MC_State = int.Parse(Request.Form["MC_State"]);
                db.SubmitChanges();
                Response.Write("修改成功!");
            }
            catch (Exception ex)
            {
                Response.Write("修改失败!错误消息:" + ex.Message);
            }
        }
        //会员换卡
        public ActionResult MemCardReplaceCard(int id)
        {
            return View((from i in db.MemCards where i.MC_ID == id select i).First());
        }
        [HttpPost]
        public void MemCardReplaceCard()
        {
            try
            {
                MemCards m = (from i in db.MemCards where i.MC_ID == int.Parse(Request.Form["MC_ID"]) select i).First();
                m.MC_CardID = Request.Form["newMcCard"];
                m.MC_Password = Request.Form["newPWD"];
            }
            catch (Exception ex)
            {
                Response.Write("异常：传值错误。详细：" + ex.Message);
            }
            try
            {
                db.SubmitChanges();
                Response.Write("换卡成功！");
            }
            catch (Exception ex)
            {
                Response.Write("异常：提交错误。详细：" + ex.Message);
            }
        }
        //会员转账
        public ActionResult MemCardTransfer(int id)
        {
            return View((from i in db.MemCards where i.MC_ID == id select i).First());
        }
        //查询会员
        [HttpPost]
        public void SearchMemCard()
        {
            if (!string.IsNullOrEmpty(Request.Form["mC_CardID"]))
            {
                var result = from i in db.MemCards where i.MC_CardID == Request.Form["mC_CardID"] select i;
                if (result.Count() > 0)
                {
                    if (result.First().MC_State == 1) Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result.First()));
                    else Response.Write("{\"msg\":\"该卡以被挂失，无法转账 \"} ");
                }
                else Response.Write("{\"msg\":\"查无此人\"} "); 
            }
            else Response.Write("{\"msg\":\"未填写卡号!\"} ");
        }
        [HttpPost]
        //会员转账
        public void MemCardTransfer()
        {
            if (!string.IsNullOrEmpty(Request.Form["TransferMoney"]) && !string.IsNullOrEmpty(Request.Form["MC_CardID"]) && !string.IsNullOrEmpty(Request.Form["OutMC_CardID"]))
            {//
                MemCards m = (from i in db.MemCards where i.MC_CardID == Request.Form["MC_CardID"] select i).First();
                MemCards out_m = (from i in db.MemCards where i.MC_CardID == Request.Form["OutMC_CardID"] select i).First();
                TransferLogs t = new TransferLogs();
                Users u = Session["UserInfo"] as Users;
                t.S_ID = u.S_ID;
                t.U_ID = u.U_ID;
                t.TL_FromMC_ID = m.MC_ID;
                t.TL_FromMC_CardID = m.MC_CardID.ToString();
                t.TL_ToMC_ID = out_m.MC_ID;
                t.TL_ToMC_CardID = out_m.MC_CardID.ToString();
                t.TL_TransferMoney = decimal.Parse(Request.Form["TransferMoney"]);
                t.TL_Remark = (Request.Form["Remark"] ?? "");
                t.TL_CreateTime = DateTime.Now;
                m.MC_Point -= Convert.ToInt32(t.TL_TransferMoney);
                out_m.MC_Point += Convert.ToInt32(t.TL_TransferMoney);
                try
                {
                    db.TransferLogs.InsertOnSubmit(t);
                    db.SubmitChanges();
                    Response.Write("转账成功！");
                }
                catch (Exception ex)
                {
                    Response.Write("异常：转账失败，错误消息：" + ex.Message);
                }
            }
            else Response.Write("异常：参数不全");
        }
        public JsonResult GetMemCardInfo()
        {
            int i = 0;
            StringBuilder sb = new StringBuilder("select * from MemCards left join CardLevels on MemCards.CL_ID = CardLevels.CL_ID  where S_ID =" + (Session["UserInfo"] as Users).S_ID + " ");
            if (!string.IsNullOrEmpty(Request.Form["mID"]))
            {
                sb.AppendFormat(" AND MC_CardID like '%{0}%'", Request.Form["mID"].Trim());i++;
            }
            if (!string.IsNullOrEmpty(Request.Form["mName"]))
            {
                sb.AppendFormat(" AND MC_Name like '%{0}%'", Request.Form["mName"].Trim()); i++;
            }
            if (!string.IsNullOrEmpty(Request.Form["mMobile"]))
            {
                sb.AppendFormat(" AND MC_Mobile like '%{0}%'", Request.Form["mMobile"].Trim()); i++;
            }
            if (!string.IsNullOrEmpty(Request.Form["cID"]))
            {
                sb.AppendFormat(" AND CL_ID = {0}", Request.Form["cID"].Trim()); i++;
            }
            if (!string.IsNullOrEmpty(Request.Form["mState"]))
            {
                sb.AppendFormat(" AND MC_State = {0}", Request.Form["mState"].Trim()); i++;
            }
            if(i == 0) sb = new StringBuilder("select * from MemCards left join CardLevels on MemCards.CL_ID = CardLevels.CL_ID  where S_ID =" + (Session["UserInfo"] as Users).S_ID + " AND MC_State = 1");
            DataTable dt = SqlHelper.GetDataTable(sb.ToString());
            List<object> ml = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                MemCards m = dr.ToModel<MemCards>();
                CardLevels c = dr.ToModel<CardLevels>();
                ml.Add(new { m = m, c = c });
            }
            int pageSize = int.Parse(Request.Form["rows"] == "NaN" ? "1": Request.Form["rows"]);
            int pageIndex = int.Parse(Request.Form["page"] == "NaN" ? "1" : Request.Form["page"]);
            return Json(new { total = ml.Count, rows = ml.Skip(--pageIndex * pageSize).Take(pageSize) }, JsonRequestBehavior.AllowGet);
        }

        // GET: MemCard/Create
        public ActionResult Create(string type, params int[] id)
        {
            ViewBag.mC_StateSel = db.CategoryItems.Where(c => c.C_Category == "MC_State").Select(c => new SelectListItem() { Text = c.CI_Name, Value = c.CI_ID.ToString() });
            ViewBag.cardLevelSel = db.CardLevels.Select(c => new SelectListItem() { Text = c.CL_LevelName, Value = c.CL_ID.ToString() });
            IEnumerable<SelectListItem> aa = db.CategoryItems.Where(c => c.C_Category == "S_Category").Select(c => new SelectListItem
            {
                Value = c.CI_ID.ToString(),
                Text = c.CI_Name
            });
            SelectList cd = new SelectList(aa.ToList(), "", "");
            if (type == "Add") return View();
            var result = (from i in db.MemCards
                          where i.MC_ID == id[0]
                          select i).First();
            return View(result);
        }
        //根据积分获取等级
        public void GetLVBYMC_Point(int id)
        {
            try
            {
                Response.Write((SqlHelper.GetScalar("select top 1 * from CardLevels where CL_NeedPoint <= " + id + " order by CL_NeedPoint desc") ?? "").ToString());
            }
            catch (Exception ex)
            {
                Response.Write("异常："+ ex.Message);
            }
        }
        //根据ID获取推荐人姓名
        public void SearchMC_RefererID(int id)
        {
            var result = from i in db.Users where i.U_ID == id select i;
            if (result.Count() > 0) Response.Write(result.First().U_RealName);
            else Response.Write("no");
        }
        //获得下一个卡号
        public void GetNextMC_CardID()
        {
            string nextCard = (SqlHelper.GetScalar("select MAX(MC_CardID) from MemCards where S_ID =" + (Session["UserInfo"] as Users).S_ID + "") ?? "").ToString();
            if (nextCard == "") nextCard = (Session["UserInfo"] as Users).S_ID + "0000";
            Response.Write(nextCard);
        }

        //公共获取值
        public MemCards GetMByForm(MemCards m)
        {
            m.S_ID = (Session["UserInfo"] as Users).S_ID;
            m.MC_CardID = Request.Form["MC_CardID"];//卡号
            m.MC_Name = Request.Form["MC_Name"];//姓名
            m.MC_Mobile = Request.Form["MC_Mobile"];//手机号码
            m.MC_Password = Request.Form["MC_Password"] == "" ? "1" : Request.Form["MC_Password"];//卡片密码
            if (!string.IsNullOrEmpty(Request.Form["MC_Sex"])) m.MC_Sex = Convert.ToBoolean(Request.Form["MC_Sex"]);//会员性别
            if (!string.IsNullOrEmpty(Request.Form["MC_BirthdayType"]))
            {
                m.MC_BirthdayType = Convert.ToBoolean(Request.Form["MC_BirthdayType"]);//会员生日
                m.MC_Birthday_Month = Convert.ToInt32(Request.Form["MC_Birthday_Month"]);//会员生日-月
                m.MC_Birthday_Day = Convert.ToInt32(Request.Form["MC_Birthday_Day"]);//会员生日-日
            }
            if (!string.IsNullOrEmpty(Request.Form["MC_IsPast"]))
            {
                m.MC_IsPast = Convert.ToBoolean(Request.Form["MC_IsPast"]=="0"?false:true);//是否设置卡片过期时间
                m.MC_PastTime = Convert.ToDateTime(Request.Form["MC_PastTime"]);//过期时间
            }
            if (!string.IsNullOrEmpty(Request.Form["MC_State"])) m.MC_State = Convert.ToInt32(Request.Form["MC_State"]);//卡片状态
            if (!string.IsNullOrEmpty(Request.Form["MC_Money"])) m.MC_Money = float.Parse(Request.Form["MC_Money"]);//卡片付费
            m.MC_Point = !string.IsNullOrEmpty(Request.Form["MC_Point"]) ? int.Parse(Request.Form["MC_Point"]) : 0;//积分数量
            if (!string.IsNullOrEmpty(Request.Form["MC_IsPointAuto"]))
            {
                m.MC_IsPointAuto = Convert.ToBoolean(Request.Form["MC_IsPointAuto"]);//积分是否可以自动转换成等级
                if (m.MC_IsPointAuto.Value && !string.IsNullOrEmpty(Request.Form["CL_ID"])) m.CL_ID = Convert.ToInt32(Request.Form["CL_ID"]);//会员等级
            }
            if (!string.IsNullOrEmpty(Request.Form["MC_RefererID"])) m.MC_RefererID = int.Parse(Request.Form["MC_RefererID"]);//推 荐 人
            if (!string.IsNullOrEmpty(Request.Form["MC_RefererName"])) m.MC_RefererName = Request.Form["MC_RefererName"];//推荐人姓名
           
            return m;
        }

        // POST: MemCard/Create
        [HttpPost]
        public void Create()
        {
            MemCards m = new MemCards();
            try
            {
                m = GetMByForm(m);
                m.MC_CreateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Response.Write("您的输入有误,请检索,错误消息:" + ex.Message);
            }

            try
            {
                db.MemCards.InsertOnSubmit(m);
                db.SubmitChanges();
                Response.Write("添加会员成功!");
            }
            catch(Exception ex)
            {
                Response.Write("添加会员有误,请检索,错误消息:" + ex.Message);
            }
        }



        // POST: MemCard/Edit/5
        [HttpPost]
        public void Edit(int id)
        {
            MemCards m = (from i in db.MemCards where i.MC_ID == id select i).First();
            try
            {
                m = GetMByForm(m);
            }
            catch (Exception ex)
            {
                Response.Write("您的输入有误,请检索,错误消息:" + ex.Message);
            }
            try
            {
                db.SubmitChanges();
                Response.Write("编辑会员成功!");
            }
            catch (Exception ex)
            {
                Response.Write("编辑会员有误,请检索,错误消息:" + ex.Message);
            }
        }

        // GET: MemCard/Delete/5
        public void Delete()
        {
            db.MemCards.DeleteOnSubmit((from i in db.MemCards where i.MC_ID == int.Parse(Request.QueryString["id"]) select i).First());
            try
            {
                db.SubmitChanges();
                Response.Write("删除成功！");
            }
            catch (Exception ex)
            {
                Response.Write("删除失败，错误消息：" + ex.Message);
            }
        }
        
        public ActionResult OutExcel()
        {
            int sID = (Session["UserInfo"] as Users).S_ID ?? 0;
            DateTime time = DateTime.Now;
            string path = "~/ExcelTemplate/会员列表_" + time.ToString("yyyy-MM-dd_hhmmss") + ".xlsx";
            try
            {
                Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();// 创建Excel工作薄
                Excel.Workbook work = excel.Workbooks.Add(Server.MapPath("~/ExcelTemplate/会员列表模板.xlsx"));//引用Excel工作簿 
                excel.Visible = true;//显示页脚
                Excel.Worksheet sheet = excel.Worksheets[1] as Excel.Worksheet;//找到第一个工作表
                sheet.Name = "会员信息表";
                var list = db.MemCards.Where(m => m.S_ID == sID).ToList();
                sheet.Cells[1, 2] = time.ToShortDateString() + "_会员列表";
                for (int i = 4, j = 2; i - 4 < list.Count; i++)
                {
                    j = 2;
                    sheet.Cells[i, j++] = list[i - 4].MC_CardID;
                    sheet.Cells[i, j++] = list[i - 4].MC_Name;
                    sheet.Cells[i, j++] = list[i - 4].MC_Sex??false ? "男" :"女";
                    sheet.Cells[i, j++] = list[i - 4].MC_Mobile;
                    sheet.Cells[i, j++] = list[i - 4].MC_CreateTime.ToString();
                    sheet.Cells[i, j++] = (from o in db.CardLevels where o.CL_ID == list[i - 4].CL_ID select o).First().CL_LevelName;
                }
                work.SaveAs(Server.MapPath(path));
                work.Close();
                excel.Quit();
                // 保存好后实现文件的下载
                FileInfo loadFile = new FileInfo(Server.MapPath(path));
                Response.Clear();
                Response.ClearHeaders();
                Response.Buffer = false;
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(loadFile.Name, System.Text.Encoding.UTF8));
                Response.AppendHeader("Content-Length", loadFile.Length.ToString());
                Response.WriteFile(loadFile.FullName);
                Response.Flush();
                //下载完文件后将excel文件夹中的临时文件删除
                System.IO.File.Delete(Server.MapPath(path));
                //释放进程
                GC.Collect();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index");
        }
        public ActionResult UpLoad()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpLoad(FormCollection form)
        {
            ViewBag.err = -1;
            int sID = (Session["UserInfo"] as Users).S_ID ?? 0;
            if (Request.Files.Count == 0) ViewBag.err = "没上传文件";
            else if (Request.Files[0].ContentLength == 0) ViewBag.err = "上传的第一个文件0字节"; 
            else
            {
                var file = Request.Files[0];
                string ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));//后缀名
                string fileName = Server.MapPath("~/UpFile/" + sID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext);//上传路劲加文件名的绝对路径
                string strConn = ext.Equals(".xls") ? string.Format(ConnString2003, fileName) : (ext.Equals(".xlsx") ? string.Format(ConnString2007, fileName) : "");//连接字符串

                if (strConn == "") ViewBag.err = "只能上传Excel2003/Excel2007格式文本";
                else
                {
                    file.SaveAs(fileName);
                    DataTable dt = new DataTable();//保存上传控件
                    using (OleDbConnection conn = new OleDbConnection(strConn))//开启一个连接对象
                    {
                        if (conn.State == ConnectionState.Closed)//关闭状态的话
                        {
                            conn.Open();//打开连接
                            try//尝试执行
                            {
                                OleDbDataAdapter odda = new OleDbDataAdapter(@"select * from [" +conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE"}).Rows[0]["TABLE_NAME"].ToString() + "]", conn);
                                odda.Fill(dt);//填充
                            }
                            catch (Exception ex)
                            {
                                ViewBag.err = "错误消息" + ex.Message;
                            }
                        }
                    }
                    try
                    {
                        foreach (DataRow r in dt.Rows)
                        {//写的是导入已导出的那种格式数据的数据
                            if (r[3].ToString().Trim() != "")//行有数据时
                            {
                                MemCards m = new MemCards();
                                m.S_ID = sID;
                                int j = 0;
                                m.MC_CardID = r[j++].ToString().Trim();
                                m.MC_Name = r[j++].ToString().Trim();
                                m.MC_Sex = r[j++].ToString().Trim() == "男" ? true : false;
                                m.MC_Mobile = r[j++].ToString().Trim();
                                m.MC_CreateTime = Convert.ToDateTime(r[j++].ToString().Trim());
                                m.CL_ID = (from i in db.CardLevels where i.CL_LevelName == r[j].ToString().Trim() select i).First().CL_ID;
                                m.MC_State = 1;
                                m.MC_Password = "123456";
                                db.MemCards.InsertOnSubmit(m);
                            }
                            db.SubmitChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.err = "错误消息" + ex.Message;
                    }
                }
            }
            return View();
            //return Index();
        }
    }
}
