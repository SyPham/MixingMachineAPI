using Microsoft.EntityFrameworkCore;
using Model;
using Model.Dto;
using Persistence;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IMachineService
    {
        IEnumerable<Machine> GetAll();
        object Add(Machine model);
        bool Update(Machine model);
        bool Delete(string id);
        object Get(string id);
    }
    public class MachineService : IMachineService
    {
        public readonly StudentDbcontext _machineDbContext;
        public MachineService(StudentDbcontext studentDbContext)
        {
            _machineDbContext = studentDbContext;
        }

        public IEnumerable<Machine> GetAll()
        {
            var result = new List<Machine>();
            try
            {
                result = _machineDbContext.machine.ToList();
            }
            catch (System.Exception)
            {

            }
            return result;

        }


        public object Get(string id)
        {
            var result = new Machine();
            try
            {
                result = _machineDbContext.machine.FirstOrDefault(x => x.machineID == id);
            }
            catch (System.Exception)
            {

            }
            return new
            {
                data = result,
                location = _machineDbContext.location
            };

        }

        public object Add(Machine model)
        {
            try
            {
                _machineDbContext.Add(model);
                _machineDbContext.SaveChanges();
            }
            catch (System.Exception)
            {

                return new { status = false };
            }
            return new { status = true, locations = _machineDbContext.location };

        }

        public bool Update(Machine model)
        {
            try
            {
                var bug = _machineDbContext.machine.Single(x => x.machineID == model.machineID);
                bug.machineID = model.machineID;
                bug.description = model.description;
                bug.locationID = model.locationID;
                _machineDbContext.Update(bug);
                _machineDbContext.SaveChanges();
            }
            catch (System.Exception)
            {

                return false;
            }
            return true;

        }


        public bool Delete(string id)
        {
            try
            {
                var item = _machineDbContext.machine.FirstOrDefault(x => x.machineID == id);
                    _machineDbContext.Remove(item);
                    _machineDbContext.SaveChanges();
            }
            catch (System.Exception)
            {

                return false;
            }
            return true;

        }

    }
}
