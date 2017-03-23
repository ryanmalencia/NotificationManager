using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using WebAPIClient.APICalls;
using DataTypes;

namespace NotificationManager
{
    class Program
    {
        static void Main(string[] args)
        {
            //SendNotifications();
            bool keepgoing = true;
            while (true)
            {
                DateTime now = DateTime.Now;
                if(now.Minute == 55 || (now.Minute - 15) == -5 || (now.Minute - 30) == -5 || (now.Minute -45) == -5)
                {
                    if (keepgoing)
                    {
                        Console.WriteLine("Starting Notifications");
                        var timer = new Timer((e) =>
                        {
                            SendNotifications();
                        }, null, 0, (int)TimeSpan.FromMinutes(15).TotalMilliseconds);
                        keepgoing = false;
                    }
                }
                else
                {
                    Console.WriteLine("Checking for appropriate start time");
                }
                Thread.Sleep(60000);
            }
        }

        public static void SendNotifications()
        {
            Console.WriteLine("Sending Notifications" + DateTime.Now.ToShortTimeString());
            List<string> campuseventtypes = CampusEventAPI.GetCampusEventTypes();
            foreach (string str in campuseventtypes)
            {
                CampusEventCollection col = new CampusEventCollection();
                if (str.Trim() != "")
                {
                    try
                    {
                        col = CampusEventAPI.GetNextHourEvents(str);
                    }
                    catch
                    {
                        col = new CampusEventCollection();
                    }
                }
                foreach (CampusEvent Event in col.Events)
                {
                    Process python = new Process();
                    python.StartInfo.FileName = @"python.exe";
                    try
                    {
                        if (Event.Organization != null && Event.Location != null)
                        {
                            if (Event.Organization.Trim() != "")
                            {
                                python.StartInfo.Arguments = string.Format("{0} \"{1}\" \"{2}\" \"{3}\" {4}", "C:\\Users\\ryanm\\Documents\\OneDrive\\Android\\notify.py", Event.Title + " - " + Event.Organization, "Today, " + Event.Time + " at " + Event.Location, "/topics/campusevent" + Event.CampusEventID, Event.CampusEventID);
                            }
                        }
                        else if (Event.Location != null)
                        {
                            python.StartInfo.Arguments = string.Format("{0} \"{1}\" \"{2}\" \"{3}\" {4}", "C:\\Users\\ryanm\\Documents\\OneDrive\\Android\\notify.py", Event.Title, "Today, " + col.Events[0].Time + " at " + Event.Location, "/topics/campusevent" + Event.CampusEventID, Event.CampusEventID);
                        }
                        else
                        {
                            python.StartInfo.Arguments = string.Format("{0} \"{1}\" \"{2}\" \"{3}\" {4}", "C:\\Users\\ryanm\\Documents\\OneDrive\\Android\\notify.py", Event.Title, "Today, " + Event.Time, "/topics/campusevent" + Event.CampusEventID, Event.CampusEventID);
                        }
                    }
                    catch(Exception)
                    {

                    }
                    python.StartInfo.UseShellExecute = true;
                    python.Start();
                    python.WaitForExit();
                }
            }
            List<string> sports = SportEventAPI.GetSportTypes();
            foreach (string str in sports)
            {
                string sport = "";
                switch (str)
                {
                    case "Women's Basketball":
                        sport = "womenbasketball";
                        break;
                    case "Men's Basketball":
                        sport = "menbasketball";
                        break;
                    case "Baseball":
                        sport = "baseball";
                        break;
                    case "Football":
                        sport = "football";
                        break;
                    case "Women's Gymnastics":
                        sport = "womengymnastics";
                        break;
                    case "Softball":
                        sport = "softball";
                        break;
                    case "Wrestling":
                        sport = "wrestling";
                        break;
                    case "Swimming & Diving":
                        sport = "swimmingdiving";
                        break;
                    }
                SportEventCollection col = new SportEventCollection();
                if (str.Trim() != "")
                {
                    try
                    {
                        col = SportEventAPI.GetNextHourEvents(str);
                    }
                    catch
                    {
                        col = new SportEventCollection();
                    }
                }
                if (str.Trim() != "" && !str.Contains("Swimming"))
                {
                    col = SportEventAPI.GetNextHourEvents(str);
                }
                foreach(SportEvent Event in col.Events)
                {
                    Process python = new Process();
                    python.StartInfo.FileName = @"python.exe";
                    try
                    {
                        python.StartInfo.Arguments = string.Format("{0} \"{1}\" \"{2}\" \"{3}\" {4}", "C:\\Users\\ryanm\\Documents\\OneDrive\\Android\\notify.py", Event.Sport.Name + " vs. " + Event.Opponent, "Today, " + String.Format("{0:t}", Event.Date) + " at " + Event.Location, "/topics/" + sport, Event.SportEventID);
                    }
                    catch (Exception)
                    {

                    }
                    python.StartInfo.UseShellExecute = true;
                    python.Start();
                    python.WaitForExit();
                }
            }
        }
    }
}
