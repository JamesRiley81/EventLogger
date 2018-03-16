using System;
using System.Collections.Generic;

namespace EventLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            int input;          
            Database database;   
            do
            {
                Console.WriteLine("Please select which task you would like to perform.\r\n\r\n1.  Parse today's file deletions.\r\n\r\n2.  View deleted files from previous days.\r\n\r\n3.  Lookup file deletions by filename.\r\n\r\n4.  Lookup file deletions by folder name.");
                try
                {
                    input = int.Parse(Console.ReadLine());
                }
                catch
                {
                    input = -1;
                }
                Console.Clear();

            } while (input != 1 && input != 2 && input != 3 && input != 4);
            switch (input)
            {
                
                case 1:
                    XmlLogger log = new XmlLogger();
                    var events = log.ParseXML();
                    if (events == null)
                    {
                        Console.WriteLine("\r\n\r\nPlease ensure you have the file in the correct location with the correct file name.\r\nOnce confirmed, press enter to reload program...");
                        Console.ReadLine();
                        System.Diagnostics.Process.Start(Environment.GetCommandLineArgs()[0]);
                        Environment.Exit(0);
                    }
                    else
                    {
                        WriteLog(events);
                        SaveLog(events);
                    }
                    break;
                case 2:
                    var start = GetStartDate();
                    var end = GetEndDate(start);
                    database = new Database();
                    var records = database.GetEvents(start, end);
                    WriteLog(records, true);
                    break;
                case 3:
                    database = new Database();
                    var recs = database.GetDeletesByName(searchString());
                    WriteLog(recs, true);
                    break;
                case 4:
                    database = new Database();
                    var rqs = database.GetDeletesByPath(searchString());
                    WriteLog(rqs, true);
                    break;
            }
        }
        static public void WriteLog(List<DeleteEvent> deletes, bool report = false)
        {
            string s;
            if (deletes.Count == 0)
                Restart();
            Console.WriteLine((deletes.Count).ToString() + " deleted files in log.  Press enter to continue...");
            Console.ReadLine();
            for (int i = 0; i < deletes.Count; i++)
            {
                Console.WriteLine((i + 1).ToString() + ". File Name: " + deletes[i].FileName + "\r\nDeleted From: " + deletes[i].FilePath + "\r\nDeleted By: " + deletes[i].User + "\tDate Deleted: " + deletes[i].Day + "\r\n\r\n");
                int count = i + 1;
                if (count%10 == 0)
                {      
                    int filesLeftCount = deletes.Count - count;
                    if (filesLeftCount == 0)
                        Console.WriteLine("End of report.  Press enter to continue.");
                    else 
                        Console.WriteLine(filesLeftCount.ToString() + " files remaining.  Press enter to continue.");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            if (report == false)
            {
                do
                {
                    Console.WriteLine("Save log to database?  Enter 'Y' for yes or 'N' for no.");
                    s = Console.ReadLine();
                    if (s.ToUpper() == "Y")
                    {
                        SaveLog(deletes);

                    }
                    else if (s.ToUpper() == "N")
                    {
                        Environment.Exit(0);
                    }
                } while (s != "Y" || s != "N");
            }
            else
            {
                do
                {
                    Console.WriteLine("Close application?  Enter 'Y' for yes or enter 'N' to restart program.");
                    s = Console.ReadLine();
                    if (s.ToUpper() == "Y")
                        Environment.Exit(0);
                    if (s.ToUpper() == "N")
                        Restart(true);
                } while (s != "Y" || s != "N");
            }
        }
        static public void SaveLog(List<DeleteEvent> deletes)
        {
            Console.WriteLine("Logging to database, please wait...");
            Database database = new Database();
            string s = string.Empty;
            if (database.SaveLog(deletes))
            {
                Console.WriteLine("Log has been written to database successfully");
                XmlLogger log = new XmlLogger();
                log.FileAwayLog();
                do
                {
                    Console.WriteLine("Close application?  Enter 'Y' for yes or enter 'N' to restart program.");
                    s = Console.ReadLine();
                    if (s.ToUpper() == "Y")
                        Environment.Exit(0);
                    if (s.ToUpper() == "N")
                        Restart(true);
                } while (s != "Y");
            }
            else
            {
                Console.WriteLine("Error in parsing log to database, contact admin for help.\r\nPress enter key to close program.");
                Console.ReadLine();
                Environment.Exit(0);
            }          
        }
        static public DateTime GetStartDate()
        {
            DateTime sdate = DateTime.MinValue;
            do
            {
                Console.WriteLine("Enter start date for deletions search in mm/dd/yyyy format.");
                try
                {
                    string s = Console.ReadLine();
                    sdate = DateTime.Parse(s);
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message + "\r\n\r\n");
                    sdate = DateTime.MinValue;
                }
            } while (sdate == DateTime.MinValue);
            return sdate;
        }
        static public DateTime GetEndDate(DateTime s)
        {
            DateTime edate = DateTime.MinValue;
            do
            {
                Console.WriteLine("\r\nEnter end date for deletions search in mm/dd/yyyy format.");
                try
                {
                    string e = Console.ReadLine();                   
                    edate = DateTime.Parse(e);
                    edate = edate.AddHours(23);
                    edate = edate.AddMinutes(59);
                    edate = edate.AddSeconds(59);
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message + "\r\n\r\n");
                    edate = DateTime.MinValue;
                }
                if (edate < s)
                {
                    Console.WriteLine("\r\n\r\nStart date is " + s.ToShortDateString() + ", end date can not be before start date");
                    edate = DateTime.MinValue;
                }
            } while (edate == DateTime.MinValue);
            return edate;
        }
        public static void Restart(bool data = false)
        {
            if (data == false)
            {
                Console.WriteLine("\r\nThere are no deletion records in this report.\r\n\r\nWould you like to run another report?\r\nEnter 'Y' for yes, otherwise either close application or press enter to close program.");
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    System.Diagnostics.Process.Start(Environment.GetCommandLineArgs()[0]);
                    Environment.Exit(0);
                }
                else
                    Environment.Exit(0);
            }
            else
                System.Diagnostics.Process.Start(Environment.GetCommandLineArgs()[0]);
            Environment.Exit(0);

        }
        public static string searchString()
        {
            string search = null;
            do
            {
                Console.Clear();
                Console.WriteLine("Enter the search value you would like to use.\r\n\r\n");
                search = Console.ReadLine();
            } while (search == string.Empty || search == null);
            return search;
        }
    }
}
