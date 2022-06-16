using JobSearchPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using System.Threading.Tasks;


namespace JobSearchPortal.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobController : ControllerBase
    {

       private JobSearchPortalContext dbcontext;
        public JobController(JobSearchPortalContext jobSearchPortalContext)
        {
            dbcontext = jobSearchPortalContext;
        }

        /// <summary>
        /// Adding New Job Access only to Admin
        /// </summary>
        /// <param name="job">Passing job details from body </param>
        /// <returns>job creatiobn</returns>
        [HttpPost]
       [Authorize(Roles = "admin")]
        public IActionResult AddJob([FromBody] Job job)
        {
            dbcontext.Jobs.Add(job);
            dbcontext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }
       
        /// <summary>
        /// Getting the avaliable Jobs Access to all the members
        /// </summary>
        /// <returns>all the jobs</returns>
        [HttpGet]
     //   [Authorize(Roles = "user")] 
        public IEnumerable<Job> Get()
        {
            return dbcontext.Jobs.Where(j => j.IsDeleted == false);
        }
        /// <summary>
        /// get job by id
        /// </summary>
        /// <param name="Jobid">job id</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetJob(int Jobid)
        {
            var data = dbcontext.Jobs.Where(x=>x.JobId==Jobid && x.IsDeleted==false);
            if (data == null)
            {
                return NotFound("Job Not Found");
                
            }
            else
            {
                return Ok(data);
            }
        }
        /// <summary>
        /// deleting job by id 
        /// accessing only to admin
        /// </summary>
        /// <param name="JobId">jobid</param>
        /// <returns>delting of job from table</returns>
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteJob(int JobId)
        {
            var data = dbcontext.Jobs.Find(JobId);
            if (data == null)
            {
                //return NotFound(" Reacod Not Found");
                return StatusCode(StatusCodes.Status204NoContent);
             //  return Request.CreateErrorResponse(System.Net.HttpStatusCode)
            }
            else
            {
                //     dbcontext.Jobs.Remove(data);
                data.IsDeleted =true;

                dbcontext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK);
               
           }

        }
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult ReAddingDeletedJob(int JobId)
        {
            var data = dbcontext.Jobs.Find(JobId);
            if (data == null)
            {
                //return NotFound(" Reacod Not Found");
                return StatusCode(StatusCodes.Status204NoContent);
            }
            else
            {
                //     dbcontext.Jobs.Remove(data);
                data.IsDeleted = false;

                dbcontext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK);

            }

        }

        /// <summary>
        /// here searching a job
        /// paging only 10 record in page
        /// </summary>
        /// <param name="JobTitle"></param>
        /// <param name="sort"></param>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        [HttpGet]
       public IActionResult SearchJob(string JobTitle, string sort, int PageNo)
        {
            var data = from Job in dbcontext.Jobs
                       where Job.JobTitle.StartsWith(JobTitle)
                       select new
                      {
                           JobId=Job.JobId,
                           JobTitle = Job.JobTitle,
                           Company = Job.CompanyName,
                           Location = Job.JobLocation,
                           JobType=Job.JobType,
                           Salary = Job.Salary,
                           Experience = Job.Experience,
                           Skills = Job.Skill,


                       };
            switch (sort)
           {
                case "desc":
                    return Ok(data.Skip((PageNo - 1) * 10).Take(10).OrderByDescending(j=>j.JobTitle));
                case "asen":
                    return Ok(data.Skip((PageNo - 1) * 10).Take(10).OrderBy(j => j.JobTitle));
                default:
                    return Ok(data.Skip((PageNo - 1) * 10).Take(10));
            }
         
        }

        [HttpGet]
        public ActionResult<IEnumerable<Job>> GetSearchedJobs(string jobTitle, string jobLocation, string jobType, int? salary, string skill,int sort, int pageno = 1)
        {
            if (sort == -1)
            {
                
                
                return (List<Job>)dbcontext.Jobs.Where(j => !j.IsDeleted &&
                (jobType == null || j.JobType == jobType) &&
                (salary == null || j.Salary <= salary) &&
                (jobLocation == null || j.JobLocation == jobLocation)).ToList()
                .Where(j => (jobTitle == null || j.JobTitle.StartsWith(jobTitle, StringComparison.OrdinalIgnoreCase))
                && (jobLocation == null || j.JobLocation.StartsWith(jobLocation, StringComparison.OrdinalIgnoreCase))
                && (skill == null || j.Skill.StartsWith(skill, StringComparison.OrdinalIgnoreCase))).ToList().
                OrderByDescending(p => p.JobId).Skip((pageno - 1) * 10).Take(10).ToList(); 
            }

            else if (sort == 1)
            {
                return (List<Job>)dbcontext.Jobs.Where(j => !j.IsDeleted &&
               (jobType == null || j.JobType == jobType) &&
               (salary == null || j.Salary <= salary) &&
               (jobLocation == null || j.JobLocation == jobLocation)).ToList()
               .Where(j => (jobTitle == null || j.JobTitle.StartsWith(jobTitle, StringComparison.OrdinalIgnoreCase))
               && (jobLocation == null || j.JobLocation.StartsWith(jobLocation, StringComparison.OrdinalIgnoreCase))
               && (skill == null || j.Skill.StartsWith(skill, StringComparison.OrdinalIgnoreCase))).ToList().
               OrderByDescending(j => j.JobId).Skip((pageno - 1) * 10).Take(10).ToList(); 
            }
            else
            {
                return (List<Job>)dbcontext.Jobs.Where(j => !j.IsDeleted &&
                (jobType == null || j.JobType == jobType) &&
                (salary == null || j.Salary <= salary) &&
                (jobLocation == null || j.JobLocation == jobLocation)).ToList()
                .Where(j => (jobTitle == null || j.JobTitle.StartsWith(jobTitle, StringComparison.OrdinalIgnoreCase))
                && (jobLocation == null || j.JobLocation.StartsWith(jobLocation, StringComparison.OrdinalIgnoreCase))
                && (skill == null || j.Skill.StartsWith(skill, StringComparison.OrdinalIgnoreCase))).ToList().
                OrderBy(j => j.Salary).Skip((pageno - 1) * 10).Take(10).ToList();
            }

        }
        [HttpGet]
        public int NumberOfSearchedProperties(string jobTitle, string jobLocation, string jobType, int? salary, string skill)
        {
            return dbcontext.Jobs.Where(j => !j.IsDeleted &&
            (jobType == null || j.JobType == jobType) &&
            (salary == null || j.Salary <= salary) &&
            (jobLocation == null || j.JobLocation == jobLocation)).ToList()
            .Where(j => (jobTitle == null || j.JobTitle.StartsWith(jobTitle, StringComparison.OrdinalIgnoreCase))
            && (jobLocation == null || j.JobLocation.StartsWith(jobLocation, StringComparison.OrdinalIgnoreCase))
            && (skill == null || j.Skill.StartsWith(skill, StringComparison.OrdinalIgnoreCase))).ToList().Count();



        }
    }
}
