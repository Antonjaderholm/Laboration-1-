using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace student
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class StudentDBEntities : DbContext
    {
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Din_Connection_String_Här");
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            // Lägg till ny student
            using (var context = new StudentDBEntities())
            {
                var student = new Student
                {
                    FirstName = "Anna",
                    LastName = "Andersson",
                    EnrollmentDate = DateTime.Now
                };

                context.Students.Add(student);
                context.SaveChanges();
            }

            // Visa alla studenter
            using (var context = new StudentDBEntities())
            {
                var students = context.Students.ToList();
                foreach (var student in students)
                {
                    Console.WriteLine($"{student.FirstName} {student.LastName}");
                }
            }
        }
    }
}


