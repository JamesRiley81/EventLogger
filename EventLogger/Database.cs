using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace EventLogger
{

    public class Database
    {
        private static OleDbConnection conn;
        private static OleDbCommand com;
        private const string CONNECTIONSTRING = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=C:\Users\JamesR\source\repos\EventLogger\EventLogger\FileDeletionLog.accdb";
        private const string FOLDERPATH = @"\\server01\Users Documents\jamesr\Documents\EventLogs\";
        public bool SaveLog(List<DeleteEvent> deletes)
        {
            try
            {
                foreach (DeleteEvent de in deletes)
                {
                    string query = "INSERT INTO FileDeletions([User], FileName, FilePath, DeletionDate) VALUES(@UN, @FileN, @FPath, @DDate)";
                    conn = new OleDbConnection(CONNECTIONSTRING);
                    com = new OleDbCommand(query, conn);
                    com.Parameters.AddWithValue("@UN", de.User);
                    com.Parameters.AddWithValue("@FileN", de.FileName);
                    com.Parameters.AddWithValue("@FPath", de.FilePath);
                    com.Parameters.AddWithValue("@DDate", DateTime.Parse(de.Day));
                    conn.Open();
                    com.ExecuteNonQuery();
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                conn.Close();
                return false;
            }
        }
        public List<DeleteEvent> GetEvents(DateTime start, DateTime end)
        {
            var events = new List<DeleteEvent>();
            try
            {
                string query = "SELECT * FROM FileDeletions WHERE DeletionDate BETWEEN @StartDate AND @EndDate ORDER BY DeletionDate";
                conn = new OleDbConnection(CONNECTIONSTRING);
                com = new OleDbCommand(query, conn);
                com.Parameters.AddWithValue("@StartDate", start);
                com.Parameters.AddWithValue("@EndDate", end);
                conn.Open();
                var read = com.ExecuteReader();
                while (read.Read())
                {
                    var dEvent = new DeleteEvent();
                    dEvent.User = read[1].ToString();
                    dEvent.FileName = read[2].ToString();
                    dEvent.FilePath = read[3].ToString();
                    dEvent.Day = read[4].ToString();
                    events.Add(dEvent);
                }
                return events;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n\r\nError when attempting to get records from database, contact admin for assistance.");
                return null;
            }
        }
        public List<DeleteEvent> GetDeletesByName(string searchName)
        {
            List<DeleteEvent> events = new List<DeleteEvent>();
            string query = "SELECT * FROM FileDeletions WHERE FileName LIKE ? ORDER BY DeletionDate";
            conn = new OleDbConnection(CONNECTIONSTRING);
            com = new OleDbCommand(query, conn);
            com.Parameters.AddWithValue("Param1", "%" + searchName + "%");
            conn.Open();
            var read = com.ExecuteReader();
            while (read.Read())
            {
                var de = new DeleteEvent();
                de.User = read[1].ToString();
                de.FileName = read[2].ToString();
                de.FilePath = read[3].ToString();
                de.Day = read[4].ToString();
                events.Add(de);
            }
            conn.Close();
            return events;
        }
        public List<DeleteEvent> GetDeletesByPath(string searchPath)
        {
            var events = new List<DeleteEvent>();
            string query = "SELECT * FROM FileDeletions WHERE FilePath LIKE ? ORDER BY DeletionDate";
            conn = new OleDbConnection(CONNECTIONSTRING);
            com = new OleDbCommand(query, conn);
            com.Parameters.AddWithValue("Param1", "%" + searchPath + "%");
            conn.Open();
            var read = com.ExecuteReader();
            while (read.Read())
            {
                var de = new DeleteEvent();
                de.User = read[1].ToString();
                de.FileName = read[2].ToString();
                de.FilePath = read[3].ToString();
                de.Day = read[4].ToString();
                events.Add(de);
            }
            conn.Close();
            return events;
        }
    }
}
