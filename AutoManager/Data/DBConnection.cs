using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace AutoManager.Data
{
    public class DBConnection
    {
        SQLiteConnection con = null;
        public void ConnectDB(string dirdata,string namedb)
        {
            string cs = @"URI=file:" + dirdata + "/" + namedb + ".db";

            con = new SQLiteConnection(cs);
            con.Open();
          
        }
        public void CreateDB(string dirdata,string namedb)
        {
            string cs = @"Data Source=" + dirdata + "/" + namedb + ".db";

            con = new SQLiteConnection(cs);
            con.Open();
        }

        public void CreateTable(string table,string coloum)
        {
            RunCommand(@"CREATE table if not exists  " + table + "(" + coloum + ")");
           
        }

      

        public List<string> RunCommand(string cmdText,int icoloum=0)
        {
            Console.WriteLine(cmdText);
            var cmd = new SQLiteCommand(cmdText, con);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            List<string> ls = new List<string>();
            while (rdr.Read())
            {
               string values = "";
               for(int i=0;i<= icoloum; i++)
               {
                    if (i != icoloum)
                    {
                        values += rdr.GetString(i) + "|";
                    }
                    else
                    {
                        values += rdr.GetString(i);
                    }
               }
                ls.Add(values);
            }
            return ls;
        }
    }
}
