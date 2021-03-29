using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> URLs0 = new List<string>(),
                         Names0 = new List<string>(),
                         States0 = new List<string>(),
                         Relates0 = new List<string>();

            SqlConnection connection = new SqlConnection(@"Data Source=WS22011\SQLEXPRESS;Initial Catalog=DMOZ; Trusted_Connection=True;");
            connection.Open();

            GetRootNodes(args[0], ref URLs0, ref Names0, ref States0, ref Relates0);

            for(int iCntr1 = 0; iCntr1 <= URLs0.Count - 1; iCntr1++)
            {
                if (States0[iCntr1] == "folder-o")
                {
                    List<string> URLs1 = new List<string>(),
                                 Names1 = new List<string>(),
                                 States1 = new List<string>(),
                                 Relates1 = new List<string>();

                    GetNodes(args[0] + URLs0[iCntr1], ref URLs1, ref Names1, ref States1, ref Relates1);
                    for (int iCntr2 = 0; iCntr2 <= URLs1.Count - 1; iCntr2++)
                    {
                        if (States1[iCntr2] == "folder-o")
                        {
                            List<string> URLs2 = new List<string>(),
                                         Names2 = new List<string>(),
                                         States2 = new List<string>(),
                                         Relates2 = new List<string>();

                            GetNodes(args[0] + URLs1[iCntr2], ref URLs2, ref Names2, ref States2, ref Relates2);
                            for (int iCntr3 = 0; iCntr3 <= URLs2.Count - 1; iCntr3++)
                            {
                                if (States2[iCntr3] == "folder-o")
                                {
                                    List<string> URLs3 = new List<string>(),
                                                 Names3 = new List<string>(),
                                                 States3 = new List<string>(),
                                                 Relates3 = new List<string>();

                                    GetNodes(args[0] + URLs2[iCntr3], ref URLs3, ref Names3, ref States3, ref Relates3);
                                    for (int iCntr4 = 0; iCntr4 <= URLs3.Count - 1; iCntr4++)
                                    {
                                        if (States3[iCntr4] == "folder-o")
                                        {
                                            List<string> URLs4 = new List<string>(),
                                                         Names4 = new List<string>(),
                                                         States4 = new List<string>(),
                                                         Relates4 = new List<string>();

                                            GetNodes(args[0] + URLs3[iCntr4], ref URLs4, ref Names4, ref States4, ref Relates4);
                                            for (int iCntr5 = 0; iCntr5 <= URLs4.Count - 1; iCntr5++)
                                            {
                                                if (States4[iCntr5] == "folder-o")
                                                {
                                                    List<string> URLs5 = new List<string>(),
                                                                 Names5 = new List<string>(),
                                                                 States5 = new List<string>(),
                                                                 Relates5 = new List<string>();

                                                    GetNodes(args[0] + URLs4[iCntr5], ref URLs5, ref Names5, ref States5, ref Relates5);
                                                    for (int iCntr6 = 0; iCntr6 <= URLs5.Count - 1; iCntr6++)
                                                    {
                                                        if (States4[iCntr5] == "folder-o")
                                                        {
                                                            //I thinks there is nothing more
                                                        }
                                                        else
                                                        {
                                                            InsertRecord(ref connection, Names0[iCntr1], Names1[iCntr2], Names2[iCntr3], Names3[iCntr4], Names4[iCntr5], Names5[iCntr6], "", Relates5[iCntr6]);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    InsertRecord(ref connection, Names0[iCntr1], Names1[iCntr2], Names2[iCntr3], Names3[iCntr4], Names4[iCntr5], "", "", Relates4[iCntr5]);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            InsertRecord(ref connection, Names0[iCntr1], Names1[iCntr2], Names2[iCntr3], Names3[iCntr4], "", "", "", Relates3[iCntr4]);
                                        }
                                    }
                                }
                                else
                                {
                                    InsertRecord(ref connection, Names0[iCntr1], Names1[iCntr2], Names2[iCntr3], "", "", "", "", Relates2[iCntr3]);
                                }
                            }
                        }
                        else
                        {
                            InsertRecord(ref connection, Names0[iCntr1], Names1[iCntr2], "", "", "", "", "", Relates1[iCntr2]);
                        }
                    }
                }
                else
                {
                    InsertRecord(ref connection, Names0[iCntr1], "", "", "", "", "", "", Relates0[iCntr1]);
                }
            }

            Console.ReadKey();
        }

        public static void InsertRecord(ref SqlConnection connection, string Level0, string Level1 = "", string Level2 = "", string Level3 = "", string Level4 = "", string Level5 = "", string Level6 = "", string RelatedCats = "")
        {
            var sql = "INSERT INTO DMOZCat(Level0, Level1, Level2, Level3, Level4, Level5, Level6, RelatedCats) VALUES(@Val0, @Val1,@Val2, @Val3, @Val4, @Val5, @Val6, @RelatedCats)";
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@Val0", Level0);
                cmd.Parameters.AddWithValue("@Val1", Level1);
                cmd.Parameters.AddWithValue("@Val2", Level2);
                cmd.Parameters.AddWithValue("@Val3", Level3);
                cmd.Parameters.AddWithValue("@Val4", Level4);
                cmd.Parameters.AddWithValue("@Val5", Level5);
                cmd.Parameters.AddWithValue("@Val6", Level6);
                cmd.Parameters.AddWithValue("@RelatedCats", RelatedCats);

                cmd.ExecuteNonQuery();
            }
        }


        public static void GetNodes(string sURL, ref List<string> URLs, ref List<string> Names, ref List<string> States, ref List<string> Relateds)
        {
            Console.WriteLine("\nConnecteing to {0} ....\n", sURL);

            string htmlCode = "",
                   sRegex = "<div class=\"cat-item\">((\\s|.)*?)</div>";

            using (WebClient client = new WebClient())
            {
                try
                {
                    htmlCode = client.DownloadString(sURL);
                }
                catch(Exception e)
                {
                  
                }
            }
            if(!string.IsNullOrEmpty(htmlCode))
                Extract(htmlCode, ref URLs, ref Names, ref States, ref Relateds, sRegex, "</i>", "<div class=\"node-count\">");
        }

        public static void GetRootNodes(string sURL, ref List<string> URLs, ref List<string> Names, ref List<string> States, ref List<string> Relateds)
        {
            Console.WriteLine("Connecteing to {0} ....\n", sURL);

            string htmlCode,
                   sRegex = "<h2 class=\"top-cat\">.*</h2>";

            using (WebClient client = new WebClient())
            {
                htmlCode = client.DownloadString(sURL);
            }
            Extract(htmlCode, ref URLs, ref Names, ref States, ref Relateds, sRegex, "/\">", "</a></h2>");
        }

        public static void Extract(string sHtml, ref List<string> URLs, ref List<string> Names, ref List<string> States, ref List<string> Relateds, string sRegex, string sBegin, string sEnd)
        {
            Dictionary<string,string> dict = new Dictionary<string,string>();
            string sTemp = ""; 

            Regex regex = new Regex(sRegex, RegexOptions.Multiline);
            if (regex.IsMatch(sHtml))
            {
                int iCntr = 0;
                string sKey, 
                       sVal,
                       sFlag,
                       sRelated = "";

                Regex regex2 = new Regex("<div class=\"cell one-browse-node\">((\\s|.)*?)</div>", RegexOptions.Multiline);
                if (regex2.IsMatch(sHtml))
                {
                    foreach (Match match2 in regex2.Matches(sHtml))
                    {
                        if (!string.IsNullOrEmpty(sRelated))
                            sRelated += ";";

                        string sTmpVal = getBetween(match2.Groups[0].Value, "<div class=\"cell one-browse-node\">", "</div>");
                        sTmpVal = sTmpVal.Replace("<i class='fa fa-chevron-right'></i>", "->");
                        sRelated += sTmpVal ;
                    }
                }

                foreach (Match match in regex.Matches(sHtml))
                {
                    sKey = getBetween(match.Groups[0].Value, "<a href=\"", "/\">");
                    sVal = getBetween(match.Groups[0].Value, sBegin, sEnd).Trim();
                    sVal = sVal.Replace("<span class=\"subtext\">", "").Replace("</span>", "").Replace("&shy;", "");
                    sFlag = getBetween(match.Groups[0].Value, "<i class='catIcon fa fa-", "'></i>").Trim();

                    if (string.IsNullOrEmpty(sFlag))
                        sFlag = "folder-o";

                    if (!string.IsNullOrEmpty(sKey) && !string.IsNullOrEmpty(sVal) && !string.IsNullOrEmpty(sFlag))
                    {
                        URLs.Add(sKey);
                        Names.Add(sVal);
                        States.Add(sFlag);
                        Relateds.Add(sRelated);

                        sTemp = ++iCntr + "- " + sFlag;
                        int iLen = 15 - sTemp.Length;
                        for (int i = 1; i < iLen; i++)
                            sTemp += "-";
                        sTemp += "> " + sVal;
                        iLen = 75 - sTemp.Length;
                        for (int i = 1; i < iLen; i++)
                            sTemp += "-";
                        sTemp += ">  " + sKey + "-->" + sRelated;

                        Console.WriteLine(sTemp);
                    }
                }
            }
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            return "";
        }
    }
}
