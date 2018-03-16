using System.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace EventLogger
{
    public class XmlLogger
    {
        private const string FOLDERPATH = @"\\server01\Users Documents\jamesr\Documents\EventLogs\";
        private const string FILEPREFIX = "Event Log ";
        private const string FILESUFFIX = ".xml";
        private const string DELETENAME = "4663";
        private const string HANDLEID = "HandleId";
        private const string EVENTID = "EventID";
        private const string NEWFOLDERPATH = @"\\server01\Users Documents\jamesr\Documents\EventLogs\ParsedLogs\";
        private string dateString = DateTime.Now.ToShortDateString();
        public List<DeleteEvent> ParseXML()
        {
            List<DeleteEvent> events = new List<DeleteEvent>();
            try
            {
                string fileString = dateString.Replace('/', '.');
                string day = string.Empty;
                XmlReader read = XmlReader.Create(FOLDERPATH + FILEPREFIX + fileString + FILESUFFIX);
                while (read.Read())
                {              
                    if (read.Name == "Event")
                    {
                        if (read.IsStartElement())
                        {
                            string nodeData = read.ReadOuterXml();
                            var xml = new XmlDocument();
                            xml.LoadXml(nodeData);
                            var theEvent = xml.GetElementsByTagName(EVENTID);
                            if (theEvent[0].InnerText == DELETENAME)
                            {
                                XmlNodeList list = xml.GetElementsByTagName("Data");
                                        var e = new DeleteEvent();
                                        var theDate = xml.GetElementsByTagName("TimeCreated");
                                        var theMessage = xml.GetElementsByTagName("Message");
                                        string message = theMessage[0].InnerText;
                                        int counter = message.IndexOf("Accesses") + 10;
                                        string accessType = message.Substring(counter, 6);
                                if (accessType == "DELETE")
                                {
                                    string t = theDate[0].OuterXml;
                                    string time = t.Substring(25, 16);
                                    var date = DateTime.Parse(time).ToLocalTime();
                                    string output = date.ToString();
                                    day = output;
                                    string user = list[1].InnerText;
                                    string path = list[6].InnerText;
                                    e.User = user;
                                    e.FilePath = path;
                                    int index = path.LastIndexOf(@"\") + 1;
                                    int count = path.Count();
                                    int fileNameCount = count - index;
                                    string file = path.Substring(index, fileNameCount);
                                    e.FileName = file;
                                    e.Day = day;
                                    events.Add(e);
                                }
                            }
                        }
                    }
                }
                read.Close();
                return events;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public bool FileAwayLog()
        {
            string fileString = dateString.Replace('/', '.');
            try
            {
                File.SetLastWriteTime(FOLDERPATH + FILEPREFIX + fileString + FILESUFFIX, DateTime.Now);
                try
                {
                    File.Move(FOLDERPATH + FILEPREFIX + fileString + FILESUFFIX, NEWFOLDERPATH + FILEPREFIX + fileString + FILESUFFIX);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "\r\n\r\nCould not archive file.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n\r\nCould not update file last modified date.");
                return false;
            }
        }
    }
}
