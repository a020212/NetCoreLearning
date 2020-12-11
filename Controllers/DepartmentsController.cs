using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPCore5Demo_1.Model;
using ASPCore5Demo_1.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ASPCore5Demo_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosoUniversityContext db;

        public DepartmentsController(ContosoUniversityContext context)
        {
            db = context;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<Department>> GetDepartmentAll()
        {
            //一般正常寫法
            //return db.Department;
            //若不須修改、追蹤相關資料狀態可以加上AsNoTracking 於DBContext的快取，記憶體會少一半占用!
            return db.Department.AsNoTracking().ToList();
        }

        [HttpGet("ddl")]
        public ActionResult<IEnumerable<DepartmentDropDown>> GetDepartmentDropDown()
        {
            // return db.Database.SqlQuery<DepartmentDropDown>($"SELECT DepartmentID, Name FROM dbo.Department").ToList();

            // return db.Departments.Select(p => new DepartmentDropDown() {
            //     DepartmentId = p.DepartmentId,
            //     Name = p.Name
            // }).ToList();

            return db.DepartmentDropDown.FromSqlInterpolated($"SELECT DepartmentID, Name FROM dbo.Department").ToList();

        }
        
        
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Course>> GetDepartmentCourse(int id)
        {
            //兩Model之前若互相關聯，進行JSON序列化可能會因循環參考造成死結，目前可用Newton JSON來進行部分欄位的忽略
            return db.Department.Include(s => s.Course).First(d => d.DepartmentId==id).Course.ToList();

            //下方為反向查詢
            //return db.Course.Where(s => s.DepartmentId == id).ToList();
            
        }
        
        [HttpPost("")]
        public ActionResult<Department> PostDepartment(Department model)
        {
            var NameParam = new SqlParameter("@Name", model.Name);
            var BudgetParam = new SqlParameter("@Budget", model.Budget);
            var StartDateParam = new SqlParameter("@StartDate", model.StartDate);
            var InstructorID = new SqlParameter("@InstructorID", model.InstructorId);

            var temp = db.Database.ExecuteSqlRaw($"Exec Department_Insert @Name,@Budget,@StartDate,@InstructorID", NameParam, BudgetParam, StartDateParam, InstructorID);
            return null;
        }

        [HttpPut("Update/{id}")]
        public IActionResult PutDepartment(int id, DepartmentUpdateModel model)
        {
            var MyDepartment = db.Department.Find(id);
            var IDParam = new SqlParameter("@DepartmentID", id);
            var NameParam = new SqlParameter("@Name", model.Name);
            var BudgetParam = new SqlParameter("@Budget", model.Budget);
            var StartDateParam = new SqlParameter("@StartDate", model.StartDate);
            var InstructorID = new SqlParameter("@InstructorId", model.InstructorId);
            var RowVerParam = new SqlParameter("@RowVersion_Original", MyDepartment.RowVersion);

            //更新更新時間
            var tempD = db.Department.Find(id);
            tempD.DateModified = System.DateTime.Now;
            db.Entry(tempD).State = EntityState.Modified;


            var temp = db.Database.ExecuteSqlRaw($"Exec Department_Update @DepartmentID,@Name,@Budget,@StartDate,@InstructorID,@RowVersion_Original", IDParam, NameParam, BudgetParam, StartDateParam, InstructorID, RowVerParam);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult<Department> DeleteDepartmentById(int id)
        {
            //更改為刪除狀態
            var temp = db.Department.Find(id);
            temp.IsDeleted = true;
            db.Entry(temp).State = EntityState.Modified;
            db.SaveChanges();

            //Call Store Procedure
            //var Department = db.Department.Find(id);
            //var IDParam = new SqlParameter("@DepartmentID", id);
            //var RowVerParam = new SqlParameter("@RowVersion_Original", Department.RowVersion);
            //var temp = db.Database.ExecuteSqlRaw($"Exec Department_Delete @DepartmentID,@RowVersion_Original", IDParam, RowVerParam);
            return null;
        }

        [HttpGet("VwDepartmentCourseCount")]
        public ActionResult<IEnumerable<VwDepartmentCourseCount>> GetVwDepartmentCourseCount()
        {
            return db.VwDepartmentCourseCount.FromSqlRaw($"Select * from VwDepartmentCourseCount").ToList();
        }

        //以下為系統產生
        //// GET: api/Departments
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        //{
        //    return await db.Department.ToListAsync();
        //}

        //// GET: api/Departments/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Department>> GetDepartment(int id)
        //{
        //    var department = await db.Department.FindAsync(id);

        //    if (department == null)
        //    {
        //        return NotFound();
        //    }

        //    return department;
        //}

        //// PUT: api/Departments/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutDepartment(int id, Department department)
        //{
        //    if (id != department.DepartmentId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(department).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DepartmentExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Departments
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Department>> PostDepartment(Department department)
        //{
        //    db.Department.Add(department);
        //    await db.SaveChangesAsync();

        //    return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        //}

        //// DELETE: api/Departments/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteDepartment(int id)
        //{
        //    var department = await db.Department.FindAsync(id);
        //    if (department == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Department.Remove(department);
        //    await db.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool DepartmentExists(int id)
        //{
        //    return db.Department.Any(e => e.DepartmentId == id);
        //}
    }
}
