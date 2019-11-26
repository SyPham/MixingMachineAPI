using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence
{
    public class StudentDbcontext:DbContext
    {
        public DbSet<Student> Student { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Rawdata> rawdata { get; set; }
        public DbSet<Location> location { get; set; }
        public DbSet<Machine> machine { get; set; }
        public StudentDbcontext(DbContextOptions<StudentDbcontext> options) :base (options)
        {

        }
    }
}
