using Microsoft.EntityFrameworkCore;
using Model;
using Model.Dto;
using Persistence;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        object GetDetail(string id, string value);
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
            var model = await _studentDbContext.rawdata.OrderByDescending(x=>x.createddatetime).FirstOrDefaultAsync(x => x.machineID == id);

            return new
            {
                model.RPM,
                display = true
            };

        }

        public object GetDetail(string id, string value)
        {
            var format = "dddd, dd MMMM yyyy hh:mm tt";
            
            var startDate = new DateTime();
            var endDate= new DateTime();
            var start = Convert.ToDateTime(startDate);
            var end = Convert.ToDateTime(endDate);
            var model = new List<MachineDetail>();
            var sequenceList = _studentDbContext.rawdata.Where(x => x.machineID == id).Select(y => new { y.createddatetime, y.sequence }).ToList();
            if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
            {
                sequenceList = sequenceList.Where(x => x.createddatetime.Date >= start.Date && x.createddatetime.Date <= end.Date).ToList();
            };

            foreach (var item in sequenceList.Distinct())
            {
                var vm = new MachineDetail();
                vm.MachineID = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.machineID).FirstOrDefault();
                vm.StartTime = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.createddatetime).Min().ToString(format);
                vm.EndTime = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.createddatetime).Max().ToString(format);
                vm.RPM = _studentDbContext.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item.sequence).Select(y => y.RPM).Average();
                model.Add(vm);
            }

            model = model.ToArray().Reverse().ToList();

            return new
            {
                model,
                standard = _studentDbContext.setting.Where(x => x.id.Equals(id)).FirstOrDefault().standardRPM
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
