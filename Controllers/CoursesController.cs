using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPCore5Demo_1.Model;
using Omu.ValueInjecter;
using ASPCore5Demo_1.Model;

namespace ASPCore5Demo_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ContosoUniversityContext db;

        public CoursesController(ContosoUniversityContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Course>> GetCourse()
        {
            return db.Course.ToList();
        }

        [HttpGet("id")]
        public ActionResult<Course> GetCourseByID(int id)
        {
            return db.Course.Find(id);
        }

        [HttpPost("")]
        public ActionResult<Course> PostCourse(Course model)
        {
            model.DateModified = System.DateTime.Now;
            db.Course.Add(model);
            db.SaveChanges();
            return Created("api/Course/" + model.CourseId,model);
        }


        [HttpPut("{id}")]
        public IActionResult PutCourse(int id, CourseUpdateModel model)
        {
            var temp = db.Course.Find(id);
            temp.DateModified = System.DateTime.Now;
            //temp.Credits = model.Credits;
            //temp.Title = model.Title;            
            temp.InjectFrom(model);

            db.SaveChanges();
            return NoContent();
        }
    
        [HttpGet("credits/{credit}")]
        public ActionResult<IEnumerable<Course>> GetCourse (int credit)
        {  
            return db.Course.Where(s => s.Credits==credit).ToList();
        }

        [HttpDelete("{id}")]
        public ActionResult<Course> DeleteCourselById(int id)
        {
            //旗標式刪除
            var temp = db.Course.Find(id);
            temp.IsDeleted = true;
            db.Entry(temp).State = EntityState.Modified;

            db.SaveChanges();


            //真的刪除
            //var temp = db.Course.Find(id);
            //db.Course.Remove(temp);
            //db.SaveChanges();

            //SQL 語法直接執行，直接避開ORM的對應，某些狀況效率較好
            // db.Database.ExecuteSqlRaw($"Delete from db.Course Where courseid={id}");
            //以下此種狀況會有SQL Injection的問題
            // db.Database.ExecuteSqlRaw($"Delete from db.Course Where courseid=" + id);
            return Ok(temp);
        }

        [HttpDelete("all")]
        public ActionResult<Course> DeleteCourseAll()
        {
            db.Database.ExecuteSqlRaw($"Delete From db.Course");
            return null;
        }

        [HttpGet("VwCourseStudents")]
        public ActionResult<IEnumerable<VwCourseStudents>> GetVwCourseStudents()
        {
            return db.VwCourseStudents.ToList();
        }

        [HttpGet("VwCourseStudentCount")]
        public ActionResult<IEnumerable<VwCourseStudentCount>> GetVwCourseStudentCount()
        {
            return db.VwCourseStudentCount.ToList();
        }


        //系統產生
        //// GET: api/Courses
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        //{
        //    return await db.Course.ToListAsync();
        //}

        //// GET: api/Courses/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Course>> GetCourse(int id)
        //{
        //    var course = await db.Course.FindAsync(id);

        //    if (course == null)
        //    {
        //        return NotFound();
        //    }

        //    return course;
        //}

        //// PUT: api/Courses/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCourse(int id, Course course)
        //{
        //    if (id != course.CourseId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(course).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CourseExists(id))
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

        //// POST: api/Courses
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Course>> PostCourse(Course course)
        //{
        //    db.Course.Add(course);
        //    await db.SaveChangesAsync();

        //    return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        //}

        //// DELETE: api/Courses/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCourse(int id)
        //{
        //    var course = await db.Course.FindAsync(id);
        //    if (course == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Course.Remove(course);
        //    await db.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool CourseExists(int id)
        //{
        //    return db.Course.Any(e => e.CourseId == id);
        //}
    }
}
