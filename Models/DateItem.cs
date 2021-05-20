// Date Item contains the volatile Archival data for the Apps date based folder and file screen
// The screen presents a Windows Explorer style display of all archived forms.
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbWebAPI.Models
{
    public class ArcItem 
    { 
        public string Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Dept { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string DayOfMonth { get; set; }
        public string DayOfWeek { get; set; }
        public string TimeOfDay { get; set; }
        public bool Year_Folder_IsVisible { get; set; }
        public bool Month_Folder_IsVisible { get; set; }
        public bool Day_Folder_IsVisible { get; set; }
        public bool Time_Folder_IsVisible { get; set; }
        public bool WarnIncompleteIsVisible { get; set; }
        public string MsgInfo { get; set; }
    }
}