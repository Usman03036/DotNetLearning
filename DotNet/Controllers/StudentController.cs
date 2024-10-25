using DotNet.Data;
using DotNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        public readonly AppDbContext DbContext;
        public StudentController(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        [Route("GetAllStudetRecords")]
        public async Task<IActionResult> GetAllStudents()
        {
            var AllStudentsRecord = await DbContext.Students.ToListAsync();
            return Ok(AllStudentsRecord);
        }

        [HttpGet]
        [Route("GetRecordById")]
        public async Task<IActionResult> GetRecordById(int id)
        {
            var FoundRecord = await DbContext.Students.FindAsync(id);
            if (FoundRecord == null)
            {
                return BadRequest("Record not found against ID: " + id);
            }

            return Ok(FoundRecord);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            if (await DbContext.Students.AnyAsync(s => s.Email == registerUserDto.Email))
            {
                return BadRequest("Email is already in use.");
            }

            // Hash the password
            var passwordHasher = new PasswordHasher<RegisterUserDto>();
            var hashedPassword = passwordHasher.HashPassword(registerUserDto, registerUserDto.Password);

            // Create a new student entity
            var student = new Student
            {
                Name = registerUserDto.Name,
                Email = registerUserDto.Email,
                Age = registerUserDto.Age,
                PasswordHash = hashedPassword,
            };

            // Save the student to the database
            await DbContext.Students.AddAsync(student);
            await DbContext.SaveChangesAsync();

            // Create a login entry linked to the student
            var login = new Login
            {
                UserId = student.Id, // Link to Student's ID
                EmailAddress = registerUserDto.Email,
                PasswordHash = hashedPassword
            };

            // Save the login info to the Login table
            await DbContext.Logins.AddAsync(login);
            await DbContext.SaveChangesAsync();

            return Ok("User registered successfully.");
        }



        [HttpPut]
        [Route("UpdateStudentRecordById")]
        public async Task<IActionResult> UpdateStudentRecordById(int Id, Student UpdatedRecordData)
        {
            var RecordToUpdate = await DbContext.Students.FindAsync(Id);
            if (RecordToUpdate == null)
            {
                return Ok("Record Not found");
            }

            RecordToUpdate.Name = UpdatedRecordData.Name;
            RecordToUpdate.Age = UpdatedRecordData.Age;
            RecordToUpdate.Email = UpdatedRecordData.Email;

            await DbContext.SaveChangesAsync();

            return Ok("Record Updated Successfully");
        }

    }
}

public class RegisterUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public string Password { get; set; }
}
