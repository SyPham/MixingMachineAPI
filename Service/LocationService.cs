using Microsoft.EntityFrameworkCore;
using Model;
using Model.Dto;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Persistence;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ILocationService
    {
        IEnumerable<Location> GetAll();
        bool Add(Location model);
        bool Update(Location model);
        bool Delete(int id);
        Location Get(int id);
        object Test(string id);
        object Get();
        Task<object> GetRPM(string id);
        object GetDetail(string id);
        object GetDetail(string id, string start ="", string end = "");
        object ExportExcel(string id, string startDate, string endDate);
    }
    public class LocationServiceService : ILocationService
    {
        public readonly StudentDbcontext _dbContext;
        public LocationServiceService(StudentDbcontext dbContext)
        {
            _dbContext = dbContext;
        }
        public object Get()
        {
            var model = from m in _dbContext.machine
                        join lc in _dbContext.location on m.locationID equals lc.id
                        select new
                        {
                            m.machineID,
                            m.description,
                            lc.locationname
                        };
            return model;
        }
        public object Test(string id)
        {
            var model = _dbContext.rawdata
                .Where(x => x.machineID == id)
                .OrderByDescending(x => x.createddatetime)
                .Select(x => x.RPM)
                .Take(30)
                .ToArray().Reverse();

            return model;

        }

        public async Task<object> GetRPM(string id)
        {
            //var machineID = new SqlParameter("machineID", id);
            //var sql = @"SELECT * FROM rawdata 
            //            WHERE machineID = @machineID 
            //            ORDER BY createddatetime DESC LIMIT 30";
            var model = await _dbContext.rawdata.OrderByDescending(x => x.createddatetime).FirstOrDefaultAsync(x => x.machineID == id);

            return new
            {
                model.RPM,
                display = true
            };

        }
        public List<MachineDetail> GetMachineDetail(string id, string startDate, string endDate)
        {
            var format = "hh:mm tt";
            var formatDate = "dddd, dd MMMM yyyy";
            var start=new DateTime();
            var end = new DateTime();
            var model = new List<MachineDetail>();
            var standard = _dbContext.setting.Where(x => x.id.Equals(id)).FirstOrDefault().standardRPM;
            var sequenceList = _dbContext.rawdata.Where(x => x.machineID == id).Select(y => new { y.createddatetime, y.sequence }).ToList();
            if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
            {
                 start = Convert.ToDateTime(startDate);
                 end = Convert.ToDateTime(endDate);
                sequenceList = sequenceList.Where(x => x.createddatetime.Date >= start.Date && x.createddatetime.Date <= end.Date).ToList();
            };
            if (!startDate.IsNullOrEmpty())
            {
                 start = Convert.ToDateTime(startDate);
                
                sequenceList = sequenceList.Where(x => x.createddatetime.Date >= start.Date ).ToList();
            };
            sequenceList = sequenceList.Where(x => x.createddatetime.Date == DateTime.Today.Date).ToList();
            foreach (var item in sequenceList.Distinct())
            {
                var max = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.createddatetime).Max();
                var min = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.createddatetime).Min();

                var vm = new MachineDetail();
                vm.MachineID = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.machineID).FirstOrDefault();
                vm.StartTime = min.ToString(format);
                vm.EndTime = max.ToString(format);
                vm.RPM = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.RPM).Average();
                vm.Date = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.createddatetime).Max().ToString(formatDate);
                vm.Minutes = max.Subtract(min).Minutes;
                vm.Status= standard < _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.RPM).Average();
                model.Add(vm);
            }

            model = model.ToArray().Reverse().Take(15).ToList();
            return model;
        }
        public object GetDetail(string id, string startDate, string endDate)
        {
            var locationID = _dbContext.machine.Where(x => x.machineID.Equals(id)).FirstOrDefault().locationID;
            return new
            {
                model = GetMachineDetail(id, startDate, endDate),
                standard = _dbContext.setting.Where(x => x.id.Equals(id)).FirstOrDefault().standardRPM,
                location = _dbContext.location.Where(x => x.id.Equals(locationID)).FirstOrDefault().locationname,
            };
        }

        public object GetDetail(string id)
        {
            id = id.ToUpper();
            var model = new List<MachineDetail>();
            var sequenceList = _dbContext.rawdata.Where(x => x.machineID == id).Select(y => y.sequence).Distinct().ToList();

            foreach (var item in sequenceList)
            {
                var vm = new MachineDetail();
                vm.MachineID = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.machineID).FirstOrDefault();
                vm.StartTime = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Min().ToString("dddd, dd MMMM yyyy hh:mm tt");
                vm.EndTime = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Max().ToString("dddd, dd MMMM yyyy hh:mm tt");
                vm.RPM = _dbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.RPM).Average();
                model.Add(vm);
            }
            model = model.ToArray().Reverse().Take(15).ToList();
            return new
            {
                model,
                standard = _dbContext.setting.Where(x => x.id.Equals(id)).FirstOrDefault()?.standardRPM
            };
        }
        public object ExportExcel(string id, string startDate, string endDate)
        {
            DataTable Dt = new DataTable();
            Dt.Columns.Add("MachineID", typeof(string));
            Dt.Columns.Add("Date", typeof(string));
            Dt.Columns.Add("Start Time", typeof(string));
            Dt.Columns.Add("End Time", typeof(string));
            Dt.Columns.Add("RPM", typeof(double));
            Dt.Columns.Add("Minutes", typeof(int));
            var model = GetMachineDetail(id, startDate, endDate);
            foreach (var item in model)
            {
                Dt.Rows.Add(item.MachineID,item.Date,item.StartTime,item.EndTime,item.RPM,item.Minutes);

            }
            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].LoadFromDataTable(Dt, true, TableStyles.None);
                worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                worksheet.DefaultRowHeight = 18;

                worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.DefaultColWidth = 20;
                worksheet.Column(2).AutoFit();

                return excelPackage.GetAsByteArray();
            }
        }
        public IEnumerable<Location> GetAll()
        {
            var result = new List<Location>();
            try
            {
                result = _dbContext.location.ToList();
            }
            catch (System.Exception)
            {

            }
            return result;

        }

        public Location Get(int id)
        {
            var result = new Location();
            try
            {
                result = _dbContext.location.Single(x => x.id == id);
            }
            catch (System.Exception)
            {

            }
            return result;

        }

        public bool Add(Location model)
        {
            try
            {
                _dbContext.Add(model);
                _dbContext.SaveChanges();
            }
            catch (System.Exception)
            {

                return false;
            }
            return true;

        }

        public bool Update(Location model)
        {
            try
            {
                var bug = _dbContext.location.Single(x => x.id == model.id);
                bug.locationname = model.locationname;
              
                _dbContext.Update(bug);
                _dbContext.SaveChanges();
            }
            catch (System.Exception)
            {

                return false;
            }
            return true;

        }


        public bool Delete(int id)
        {
            try
            {
                _dbContext.Entry(new Location { id = id }).State = EntityState.Deleted; ;
                _dbContext.SaveChanges();
            }
            catch (System.Exception)
            {

                return false;
            }
            return true;

        }

    }
}
