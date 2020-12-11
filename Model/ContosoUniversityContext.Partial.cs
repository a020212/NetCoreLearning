using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCore5Demo_1.Model
{
    public partial class ContosoUniversityContext : DbContext
    {

        public virtual DbSet<DepartmentDropDown> DepartmentDropDown { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentDropDown>(entity =>
            {
                entity.HasNoKey();
            });
        }
    }
}
