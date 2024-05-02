using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Utility.DataAccessUtility;

namespace WhatsAppIntegration_CPC
{
    public class Program
    {
        public string ErrorMessage;
        public string respJSON;


        public string ErrorStatus;
        public string msgDtl;
        static void Main(string[] args)
        {
            Program obj = new Program();

            string SendMsgURL = "https://media.smsgupshup.com/GatewayAPI/rest";

            Task tc;

            tc = Task.Run(() => obj.SendWhatsAppMsg(SendMsgURL));

            tc.Wait();
        }

        public async System.Threading.Tasks.Task SendWhatsAppMsg(string SendMsgURL)
        {



            // List<MessageJsonDetails> MsgListObj = new List<MessageJsonDetails>();
            MessageJsonDetails msgObj = new MessageJsonDetails();

            OptinDetails OptinObj = new OptinDetails();

            string spname = "";

            string cmdUserDtl = @"select distinct ID as UserID,Employee_Code,Employee_Name ,
                StoreID,RoleID,ManagerID,Emp_MobileNo,Emp_Channel from EmployeeMaster 
                where SyncStatus=1 and LTRIM(RTRIM(Emp_Channel))='CPC'   and (Emp_MobileNo is not null or Emp_MobileNo<>'')";// and ID=14966,14818";
            DataSet dsDtl = SqlHelper.ExecuteDataset(SqlHelper.ConnectionString(), CommandType.Text, cmdUserDtl);
            if (dsDtl != null && dsDtl.Tables.Count > 0 && dsDtl.Tables[0].Rows.Count > 0)
            {
                //MailMessage Msg = new MailMessage();
                //// Sender e-mail address.
                //Msg.From = new MailAddress("bkmistcopia@gmail.com");

                //Msg.To.Add("Rasmita.Sahu@copiacs.com,sawan.subudhi@vipbags.com,srikanth.macha@vipbags.com");

                //Msg.Subject = "Promoters and Manager Whats App message data " + (DateTime.Now.Date.Day) + "_" + (DateTime.Now.Date.Month) + "_" + (DateTime.Now.Date.Year);
                //Msg.Body = "\nDear Sir, \n\nWhats App message of Promoters and Manager data has been initiated.  \n\n\n" +
                //    "With Regards,\nSFA Support Team ";

                //SmtpClient smtp = new SmtpClient();
                //smtp.Host = "smtp.gmail.com";
                //smtp.Port = 587;
                //smtp.Credentials = new System.Net.NetworkCredential("bkmistcopia@gmail.com", "yleijjpypwnxirta");
                //smtp.EnableSsl = true;
                //smtp.Send(Msg);
                //smtp.Dispose();
                //Msg = null;

                for (int i = 0; i < dsDtl.Tables[0].Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dsDtl.Tables[0].Rows[i]["Emp_MobileNo"].ToString()))
                    {
                        string UserID = dsDtl.Tables[0].Rows[i]["UserID"].ToString();
                        string RoleID = dsDtl.Tables[0].Rows[i]["RoleID"].ToString();
                        string Channel = dsDtl.Tables[0].Rows[i]["Emp_Channel"].ToString();
                        string Emp_MobileNo = dsDtl.Tables[0].Rows[i]["Emp_MobileNo"].ToString();
                        string ManagerID = dsDtl.Tables[0].Rows[i]["ManagerID"].ToString();
                        string StoreID = dsDtl.Tables[0].Rows[i]["StoreID"].ToString();

                        ErrorStatus = "";
                        msgDtl = "";

                        OptinObj.method = "OPT_IN";
                        OptinObj.userid = "2000217099";
                        OptinObj.password = "CwxCsH9Q";
                        OptinObj.phone_number = dsDtl.Tables[0].Rows[i]["Emp_MobileNo"].ToString();// dsDtl.Tables[0].Rows[i]["Emp_MobileNo"].ToString();//" 8657922856,8336815605";
                        OptinObj.v = "1.1";
                        OptinObj.auth_scheme = "plain";
                        OptinObj.channel = "WHATSAPP";

                        string jsondataOpt = new JavaScriptSerializer().Serialize(OptinObj);

                        jsondataOpt = jsondataOpt.Replace("[", "").Replace("]", "");

                        HttpResponseMessage responseOpt;
                        string resultOpt = string.Empty;
                        string AcceptOpt = string.Empty;
                        try
                        {

                            using (HttpClient clientOpt = new HttpClient())
                            {
                                clientOpt.BaseAddress = new Uri(SendMsgURL);

                                clientOpt.DefaultRequestHeaders.Accept.Clear();
                                AcceptOpt = "application/json";
                                var httpContentOpt = new StringContent(jsondataOpt.ToString(), System.Text.Encoding.UTF8, AcceptOpt);

                                httpContentOpt.Headers.ContentType.CharSet = null;

                                responseOpt = await clientOpt.PostAsync("", httpContentOpt);

                                resultOpt = await responseOpt.Content.ReadAsStringAsync();

                                if (responseOpt.IsSuccessStatusCode == false)
                                {


                                }
                                else
                                {
                                    msgObj.userid = "2000217099";
                                    msgObj.password = "CwxCsH9Q";
                                    msgObj.send_to = dsDtl.Tables[0].Rows[i]["Emp_MobileNo"].ToString();// dsDtl.Tables[0].Rows[i]["Emp_MobileNo"].ToString();//"8452837067";
                                    msgObj.v = "1.1";
                                    msgObj.format = "json";
                                    msgObj.msg_type = "TEXT";
                                    msgObj.method = "SENDMESSAGE";
                                    if (!string.IsNullOrEmpty(dsDtl.Tables[0].Rows[i]["RoleID"].ToString()))
                                    {
                                        if (dsDtl.Tables[0].Rows[i]["RoleID"].ToString() == "37") // Promoter
                                        {
                                            //spname = "[proc_WhatsappMSG_Promoter_16/10/2023]"; // next morning 8am

                                            spname = "[proc_WhatsappMSGCPC_Promoter_7PM]"; // Same Day 7pm
                                            SqlParameter[] parameter = new SqlParameter[1];
                                            parameter[0] = new SqlParameter("@UserID", string.IsNullOrEmpty(dsDtl.Tables[0].Rows[i]["UserID"].ToString()) ? "" : dsDtl.Tables[0].Rows[i]["UserID"].ToString());

                                            DataSet dsPromoter = SqlHelper.ExecuteDataset(SqlHelper.ConnectionString(), CommandType.StoredProcedure, spname, parameter);
                                            if (dsPromoter != null && dsPromoter.Tables.Count > 0 && dsPromoter.Tables[0].Rows.Count > 0)
                                            {

                                                msgObj.msg = "Channel+%3A+CPC%0ADaily+Tracker+%28P%29+for+" + dsPromoter.Tables[0].Rows[0]["CreatedDate_dd"].ToString() + "%2F" + dsPromoter.Tables[0].Rows[0]["CreatedDate_mm"].ToString() + "%0AAttendance+%3A+" + dsPromoter.Tables[0].Rows[0]["AttendanceStatus"].ToString() + "%0ATertiary+Status+%3A+" + dsPromoter.Tables[0].Rows[0]["Tertiary_Status"].ToString() + "%0AFTD+Sales+%3A+" + dsPromoter.Tables[0].Rows[0]["tertiaryMTDsaleFTD"].ToString() +
                                                    "%0AMTD+%28Tgt%2FAch%2FAch%25%29+%3A+" + dsPromoter.Tables[0].Rows[0]["tertiaryTargetValue"].ToString() + "/" + dsPromoter.Tables[0].Rows[0]["tertiaryMTDsale"].ToString() + "/" + dsPromoter.Tables[0].Rows[0]["tertiaryAchievementPercentage"].ToString() + "%0A%0AThis+is+an+autogenerated+message.+Kindly+do+not+replay+back.";

                                                if (msgObj != null)
                                                {
                                                    string jsondata = new JavaScriptSerializer().Serialize(msgObj);


                                                    jsondata = jsondata.Replace("[", "").Replace("]", "");

                                                    HttpResponseMessage response;
                                                    string result = string.Empty;
                                                    string Accept = string.Empty;
                                                    try
                                                    {

                                                        using (HttpClient client = new HttpClient())
                                                        {
                                                            client.BaseAddress = new Uri(SendMsgURL);

                                                            client.DefaultRequestHeaders.Accept.Clear();
                                                            Accept = "application/json";
                                                            var httpContent = new StringContent(jsondata.ToString(), System.Text.Encoding.UTF8, Accept);

                                                            httpContent.Headers.ContentType.CharSet = null;

                                                            response = await client.PostAsync("", httpContent);

                                                            result = await response.Content.ReadAsStringAsync();

                                                            if (response.IsSuccessStatusCode == false)
                                                            {
                                                                ErrorMessage = result;
                                                                ErrorStatus = "Failed";
                                                                msgDtl = result;
                                                            }
                                                            else
                                                            {
                                                                respJSON = result;
                                                                ErrorStatus = "Success";
                                                                msgDtl = result;
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    finally
                                                    {
                                                        response = null/* TODO Change to default(_) if this is not a reference type */;
                                                        result = null;

                                                    }
                                                }


                                            }
                                        }
                                        else
                                        { //Manager
                                            //spname = "[proc_WhatsappMSG_Manager_16/10/2023]"; // next morning 8am

                                            spname = "[proc_WhatsappMSGCPC_Manager_7PM]"; // Same Day 7pm
                                            SqlParameter[] parameter = new SqlParameter[1];

                                            parameter[0] = new SqlParameter("@UserID", string.IsNullOrEmpty(dsDtl.Tables[0].Rows[i]["UserID"].ToString()) ? "" : dsDtl.Tables[0].Rows[i]["UserID"].ToString());
                                            DataSet dsManager = SqlHelper.ExecuteDataset(SqlHelper.ConnectionString(), CommandType.StoredProcedure, spname, parameter);
                                            if (dsManager != null && dsManager.Tables.Count > 0 && dsManager.Tables[0].Rows.Count > 0)
                                            {


                                                msgObj.msg = "Channel+%3A+CPC%0ADaily+Tracker+%28M%29+for+" + dsManager.Tables[0].Rows[0]["CreatedDate_dd"].ToString() + "%2F" + dsManager.Tables[0].Rows[0]["CreatedDate_mm"].ToString() +
                                                    "%0AAttendance+%3A+" + dsManager.Tables[0].Rows[0]["PromoterCntByDate"].ToString() + "+|+" + dsManager.Tables[0].Rows[0]["PresentCntByDate"].ToString() + "+|+" + dsManager.Tables[0].Rows[0]["AbsentCntByDate"].ToString() + "+|+" + dsManager.Tables[0].Rows[0]["WeeklyOffMarkCntByDate"].ToString() +
                                                    "%0ATertiary+Status+%3A+" + dsManager.Tables[0].Rows[0]["DSR_Filed"].ToString() + "+Filled /+" + dsManager.Tables[0].Rows[0]["DSR_Notfiled"].ToString() + "+Not+Filled " +
                                                    "%0AFTD+Sales+%3A+" + dsManager.Tables[0].Rows[0]["FtdSale"].ToString() + "%0AMTD+%28Tgt%2FAch%2FAch%25%29+%3A+" + dsManager.Tables[0].Rows[0]["tertiaryTargetValue"].ToString() + "%2F" + dsManager.Tables[0].Rows[0]["tertiaryMTDsale"].ToString() +
                                                    "%2F" + dsManager.Tables[0].Rows[0]["tertiaryAchievementPercentage"].ToString() + "%0A%0AThis+is+an+autogenerated+message.+Kindly+do+not+replay+back.";
                                                //MsgListObj.Add(msgObj);  

                                                if (msgObj != null)
                                                {
                                                    string jsondata = new JavaScriptSerializer().Serialize(msgObj);


                                                    jsondata = jsondata.Replace("[", "").Replace("]", "");

                                                    HttpResponseMessage response;
                                                    string result = string.Empty;
                                                    string Accept = string.Empty;
                                                    try
                                                    {

                                                        using (HttpClient client = new HttpClient())
                                                        {
                                                            client.BaseAddress = new Uri(SendMsgURL);

                                                            client.DefaultRequestHeaders.Accept.Clear();
                                                            Accept = "application/json";
                                                            var httpContent = new StringContent(jsondata.ToString(), System.Text.Encoding.UTF8, Accept);

                                                            httpContent.Headers.ContentType.CharSet = null;

                                                            response = await client.PostAsync("", httpContent);

                                                            result = await response.Content.ReadAsStringAsync();

                                                            if (response.IsSuccessStatusCode == false)
                                                            {
                                                                ErrorMessage = result;
                                                                ErrorStatus = "Failed";
                                                                msgDtl = result;
                                                            }
                                                            else
                                                            {
                                                                respJSON = result;
                                                                ErrorStatus = "Success";
                                                                msgDtl = result;
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    finally
                                                    {
                                                        response = null/* TODO Change to default(_) if this is not a reference type */;
                                                        result = null;

                                                    }
                                                }


                                            }
                                        }
                                    }


                                    if (!string.IsNullOrEmpty(ErrorStatus))
                                    {
                                        string InsCmd = @"INSERT INTO [dbo].[WhatsAppMsgDetails_Tbl] ([UserID] ,[RoleID] ,[Channel] ,[Emp_MobileNo]
                                                                               ,[ManagerID] ,[StoreID] ,[WhatsAppMsg] ,[MsgGoBy] ,[CreatedDate] ,[MsgStatus],[ErrorMsg])
                                                                                VALUES(" + UserID + "," + RoleID + ",'" + Channel + "'," + Emp_MobileNo + "," + ManagerID + "," + StoreID + "," +
                                                                               "'" + msgObj.msg + "','Scheduler',getdate(),'" + ErrorStatus + "','" + msgDtl + "')";
                                        int RowAffect = SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionString(), CommandType.Text, InsCmd);
                                    }


                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {
                            responseOpt = null/* TODO Change to default(_) if this is not a reference type */;
                            resultOpt = null;

                        }




                    }


                }

            }






            //for (int i = 0; i < 1; i++)
            //{
            //    msgObj = new MessageJsonDetails();
            //    msgObj.userid = "2000217099";
            //    msgObj.password = "CwxCsH9Q";
            //    msgObj.send_to = "8452837067";
            //    msgObj.v = "1.1";
            //    msgObj.format = "json";
            //    msgObj.msg_type = "TEXT";
            //    msgObj.method = "SENDMESSAGE";
            //    //msgObj.msg = "Welcome+To+VIP+Industry";
            //    //msgObj.msg = "Date+-+a123%0AAttendances+Status+-+b234%0ADSR+Status+-+c789%0ASales+Qty+Pcs+-+d698%0ATodays+Target+-+e369%0AAchieved+Value+-+8000%0AShort+Fall%2F+Excess+Sales+-+g624%0AAchieved+%25+-+h741%0A%0AMonthly+Target+%28In+Lacs%29+-+i2356%0AMTD+Sales+%28In+Lacs%29+-+j862%0AAchievement+%25+-+50";
            //    msgObj.msg = "Date+-+a123%0AAttendance+Status+-+b234%0ADSR+Status+-+c789%0ASales+Qty+Pcs+-+d698%0ATodays+Target+-+e369%0AAchieved+Value+-+8000%0AShort+Fall%2F+Excess+Sales+-+g624%0AAchieved+%25+-+h741%0A%0AMonthly+Target+%28In+Lacs%29+-+i2356%0AMTD+Sales+%28In+Lacs%29+-+j862%0AAchievement+%25+-+50";
            //    msgObj.isTemplate = "true";
            //    msgObj.header = "SFA+Promoter";

            //    MsgListObj.Add(msgObj);
            //}

            //string jsondata = new JavaScriptSerializer().Serialize(MsgListObj);

            //jsondata= jsondata.Replace("[", "").Replace("]", "");

            //HttpResponseMessage response;
            //string result = string.Empty;
            //string Accept = string.Empty;
            //try
            //{
            //    // Serialize our concrete class into a JSON String
            //    //var stringPayload = JsonConvert.SerializeObject(MsgListObj);


            //    //var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            //    //var httpClient = new HttpClient();

            //    // var httpResponse = await httpClient.PostAsync(SendMsgURL, httpContent);


            //    //if (httpResponse.Content != null)
            //    //{
            //    //    var responseContent = await httpResponse.Content.ReadAsStringAsync();


            //    //}
            //    using (HttpClient client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(SendMsgURL);

            //        client.DefaultRequestHeaders.Accept.Clear();
            //        Accept = "application/json"; 
            //        var httpContent = new StringContent(jsondata.ToString(), System.Text.Encoding.UTF8, Accept);

            //        httpContent.Headers.ContentType.CharSet = null;

            //        response = await client.PostAsync("", httpContent);

            //        result = await response.Content.ReadAsStringAsync();

            //        if (response.IsSuccessStatusCode == false)
            //        {
            //            ErrorMessage = result;


            //        }
            //        else
            //        {
            //            respJSON = result;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            //finally
            //{
            //    response = null/* TODO Change to default(_) if this is not a reference type */;
            //    result = null;

            //}

        }
    }
}
