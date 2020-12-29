using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Dapper;
using DN.NSC.RecentRepairs.Properties;
using DN.NSC.RecentRepairs.util;

namespace DN.NSC.RecentRepairs
{
    public class ViewModel : ViewModelBase
    {
        private Repair _repair;
        private ObservableCollection<Repair> _repairs;
        public Repair Repair
        {
            get => _repair;
            set
            {
                _repair = value;
                NotifyPropertyChanged(nameof(Repair));
            }
        }
        public ObservableCollection<Repair> Repairs
        {
            get => _repairs;
            set
            {
                _repairs = value;
                NotifyPropertyChanged(nameof(Repairs));
            }
        }

        private TotalRepair _totalRepair;
        private ObservableCollection<TotalRepair> _totalRepairs;

        public TotalRepair TotalRepair
        {
            get => _totalRepair;
            set
            {
                _totalRepair = value;
                NotifyPropertyChanged(nameof(TotalRepair));
            }
        }
        public ObservableCollection<TotalRepair> TotalRepairs
        {
            get => _totalRepairs;
            set
            {
                _totalRepairs = value;
                NotifyPropertyChanged(nameof(TotalRepairs));
            }
        }

        void Repairs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Repairs));
        }

        void TotalRepairs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TotalRepairs));
        }
        public ViewModel()
        {
            DispatcherTimer dispatchTimer = new DispatcherTimer();
            dispatchTimer.Tick += dispatchTimer_Tick;
            dispatchTimer.Interval = new TimeSpan(0, 1, 0);
            dispatchTimer.Start();

            Repair = new Repair();
            Repairs = new ObservableCollection<Repair>();
            Repairs.CollectionChanged += Repairs_CollectionChanged;
            TotalRepair = new TotalRepair();
            TotalRepairs = new ObservableCollection<TotalRepair>();
            TotalRepairs.CollectionChanged += TotalRepairs_CollectionChanged;
            Update();
        }
        public void dispatchTimer_Tick(object sender, EventArgs e)
        {
            Update();
        }

        public List<Repair> FetchRepairsListFromDB()
        {
            var query = @"SELECT Job_CDate as DateAdded,
                        Call_Employ_Num as EngineerNumber,
                        Employ_Name as EngineerName,
                        Call_Ser_Num as SerialNumber,
                        isnull(Part_Desc, Prod_Desc) as ProductDescription
                        from COOPESOLBRANCHLIVE.dbo.SCCall
                        LEFT JOIN COOPESOLBRANCHLIVE.dbo.SCPART on isnull(Call_Prod_Num,Job_Part_Num) = Part_Num
                        LEFT JOIN COOPESOLBRANCHLIVE.dbo.SCPROD on isnull(Call_Prod_Num,Job_Part_Num) = Prod_Num
                        LEFT JOIN COOPESOLBRANCHLIVE.dbo.SCEMPLOY ON Call_Employ_Num = Employ_Num
                        WHERE datediff(day,Job_CDate,getdate()) = 0
                        group by Job_CDate, Call_Ser_Num, isnull(Part_Desc, Prod_Desc), Call_Employ_Num, Employ_Name
                        order by Job_CDate DESC, Call_Ser_Num";
            var _ = Convert.FromBase64String("RGF0YSBTb3VyY2U9MTAuMTIxLjY4LjY2XENPT1BFU09MQlJBTkNITElWRSwxODM3OyBJbnRlZ3JhdGVkIFNlY3VyaXR5PUZhbHNlO1VzZXIgSUQ9dGVzc2VyYWN0O1Bhc3N3b3JkPVRlNTVlcmFjdA==");
            var connection = new SqlConnection(Encoding.UTF8.GetString(_));
            var update = connection.Query<Repair>(query).ToList();
            return update;
        }

        public List<TotalRepair> FetchTotalRepairsFromDB()
        {
            var query = @"SELECT 
            'Name' = Employ_Name,
            'Total' = ISNULL(COUNT(Call_Num),0)
             FROM COOPESOLBRANCHLIVE.dbo.SCCall
             JOIN COOPESOLBRANCHLIVE.dbo.SCEmploy ON Call_Employ_Num = Employ_Num
             WHERE Job_CDate between Convert(DateTime, DATEDIFF(DAY, 0, GETDATE())) and Dateadd(day, 1, DATEDIFF(DAY, 0, GETDATE()))
             and Employ_Para like '%BK'
             group by Employ_Name
             order by Employ_Name";
            var _ = Convert.FromBase64String("RGF0YSBTb3VyY2U9MTAuMTIxLjY4LjY2XENPT1BFU09MQlJBTkNITElWRSwxODM3OyBJbnRlZ3JhdGVkIFNlY3VyaXR5PUZhbHNlO1VzZXIgSUQ9dGVzc2VyYWN0O1Bhc3N3b3JkPVRlNTVlcmFjdA==");
            var connection = new SqlConnection(Encoding.UTF8.GetString(_));
            var update = connection.Query<TotalRepair>(query).ToList();
            return update;
        }
        
        public void Update()
        {
            var Results = FetchRepairsListFromDB();
            foreach (Repair uRepair in Results){
                if (Repairs.All(x => x.SerialNumber != uRepair.SerialNumber))
                    Repairs.Add(uRepair);
            }

            Repairs.OrderByDescending(x=> x.DateAdded);
            TotalRepairs = new ObservableCollection<TotalRepair>(FetchTotalRepairsFromDB());
            TotalRepairs.OrderBy(x => x.Name);
        }

    }
}