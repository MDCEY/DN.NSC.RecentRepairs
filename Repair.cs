using System;

namespace DN.NSC.RecentRepairs
{
    public class Repair
    {
        public DateTime DateAdded { get; set; }
//        public string EngineerNumber { get; set; }
        public string EngineerName { get; set; }
        public string ProductDescription { get; set; }
        public string SerialNumber { get; set; }
    }
}