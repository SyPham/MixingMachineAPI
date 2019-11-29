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
    public interface IStudentService
    {
        IEnumerable<Student> GetAll();
        bool Add(Student model);
        bool Update(Student model);
        bool Delete(int id);
        Student Get(int id);
        object Test(string id);
        object Get();
        Task<object> GetRPM(string id);
        object GetDetail(string id);
        object GetDetail(string id, string start = "", string end = "");
        object ExportExcel(string id, string startDate, string endDate);
    }
    public class StudentService : IStudentService
    {
        public readonly StudentDbcontext _studentDbContext;
        public StudentService(StudentDbcontext studentDbContext)
        {
            _studentDbContext = studentDbContext;
        }
        public object Get()
        {
            var model = from m in _studentDbContext.machine
                        join lc in _studentDbContext.location on m.locationID equals lc.id
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
            var model = _studentDbContext.rawdata
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
            var model = await _studentDbContext.rawdata.OrderByDescending(x => x.createddatetime).FirstOrDefaultAsync(x => x.machineID == id);

            return new
            {
                model.RPM,
                display = true
            };

        }
        public List<MachineDetail> GetMachineDetail(string id, string startDate, string endDate)
        {
            var format = "hh:mm:ss tt";
            var formatDate = "dddd, dd MMMM yyyy";
            var start = new DateTime();
            var end = new DateTime();
            var model = new List<MachineDetail>();
            var standard = _studentDbContext.setting.Where(x => x.id.Equals(id))
                                                    .FirstOrDefault().standardRPM;
            var sequenceList = _studentDbContext.rawdata.Where(x => x.machineID == id)
                .Select(y => new
                {
                    y.createddatetime,
                    y.sequence
                })
                .ToList();
            if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
            {
                start = Convert.ToDateTime(startDate);
                end = Convert.ToDateTime(endDate);
                sequenceList = sequenceList.Where(x => x.createddatetime.Date >= start.Date && x.createddatetime.Date <= end.Date).ToList();
            }
            else if (!startDate.IsNullOrEmpty())
            {
                start = Convert.ToDateTime(startDate);

                sequenceList = sequenceList.Where(x => x.createddatetime.Date >= start.Date).ToList();
            }
            else if (!endDate.IsNullOrEmpty())
            {
                end = Convert.ToDateTime(endDate);

                sequenceList = sequenceList.Where(x => x.createddatetime.Date <= end.Date).ToList();
            }
            else
            {
                sequenceList = sequenceList.Where(x => x.createddatetime.Date == DateTime.Now.Date).ToList();
            };
            int count = 1;
            foreach (var item in sequenceList.Select(x => x.sequence).Distinct())
            {
                var max = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Max();
                var min = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Min();
                var totalMinutes = Math.Round(TimeSpan.FromTicks(max.Ticks - min.Ticks).TotalMinutes,2);
                var totalSeconds = TimeSpan.FromTicks(max.Ticks - min.Ticks).TotalSeconds;
                var vm = new MachineDetail();
                vm.No = count;
                vm.MachineID = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.machineID).FirstOrDefault();
                vm.StartTime = min.ToString(format);
                vm.EndTime = max.ToString(format);
                vm.RPM = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.RPM).Average();
                vm.Date = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Max().ToString(formatDate);
                vm.Minutes = totalMinutes;
                vm.Second = (int)totalSeconds;
                vm.Status = standard < _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.RPM).Average();
                model.Add(vm);
                count++;
            }

            model = model.ToArray().Reverse().Take(30).ToList();
            return model;
        }
        public object GetDetail(string id, string startDate, string endDate)
        {
            var locationID = _studentDbContext.machine.Where(x => x.machineID.Equals(id)).FirstOrDefault().locationID;
            return new
            {
                model = GetMachineDetail(id, startDate, endDate),
                standard = _studentDbContext.setting.Where(x => x.id.Equals(id)).FirstOrDefault().standardRPM,
                minstandard = _studentDbContext.setting.Where(x => x.id.Equals(id)).FirstOrDefault().minRPM,
                location = _studentDbContext.location.Where(x => x.id.Equals(locationID)).FirstOrDefault().locationname,
            };
        }

        public object GetDetail(string id)
        {
            id = id.ToUpper();
            var model = new List<MachineDetail>();
            var sequenceList = _studentDbContext.rawdata.Where(x => x.machineID == id).Select(y => y.sequence).Distinct().ToList();

            foreach (var item in sequenceList)
            {
                var vm = new MachineDetail();
                vm.MachineID = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.machineID).FirstOrDefault();
                vm.StartTime = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Min().ToString("dddd, dd MMMM yyyy hh:mm tt");
                vm.EndTime = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Max().ToString("dddd, dd MMMM yyyy hh:mm tt");
                vm.RPM = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.RPM).Average();
                model.Add(vm);
            }
            model = model.ToArray().Reverse().Take(15).ToList();
            return new
            {
                model,
                standard = _studentDbContext.setting.Where(x => x.id.Equals(id)).FirstOrDefault()?.standardRPM
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
                Dt.Rows.Add(item.MachineID, item.Date, item.StartTime, item.EndTime, item.RPM, item.Minutes);

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
        public IEnumerable<Student> GetAll()
        {
            var result = new List<Student>();
            try
            {
                result = _studentDbContext.Student.ToList();
            }
            catch (System.Exception)
            {

            }
            return result;

        }

        public Student Get(int id)
        {
            var result = new Student();
            try
            {
                result = _studentDbContext.Student.Single(x => x.StudentId == id);
            }
            catch (System.Exception)
            {

            }
            return result;

        }

        public bool Add(Student model)
        {
            try
            {
                _studentDbContext.Add(model);
                _studentDbContext.SaveChanges();
            }
            catch (System.Exception)
            {

                return false;
            }
            return true;

        }

        public bool Update(Student model)
        {
            try
            {
                var bug = _studentDbContext.Student.Single(x => x.StudentId == model.StudentId);
                bug.Name = model.Name;
                bug.LastName = model.LastName;
                bug.Bio = model.Bio;
                _studentDbContext.Update(bug);
                _studentDbContext.SaveChanges();
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
                _studentDbContext.Entry(new Student { StudentId = id }).State = EntityState.Deleted; ;
                _studentDbContext.SaveChanges();
            }
            catch (System.Exception)
            {

                return false;
            }
            return true;

        }

    }
}
