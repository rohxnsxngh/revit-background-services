using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using BackgroundServices.Models;
using Hangfire;
using BackgroundServices.Services;
using System.Collections.Generic;
using BackgroundServices.Migrations;
using System.Text;
using System.Threading;

namespace BackgroundServices.Controllers
{
    [ApiController]
    [Route("background-service/")]
    public class SyncModelController : ControllerBase
    {
        private static List<SyncModel> models = new List<SyncModel>();
        private readonly ILogger<SyncModelController> _logger;
        private readonly IServiceManagement _serviceManagement;

        public SyncModelController(ILogger<SyncModelController> logger, IServiceManagement serviceManagement)
        {
            _logger = logger;
            _serviceManagement = serviceManagement;
        }

        [HttpPost]
        [Route("generate")]
        [ActionName(nameof(GenerateJob))]
        public IActionResult GenerateJob(SyncModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    models.Add(model);

                    var revitModelNames = model.model_name;
                    var userName = model.user_name;
                    model.status = 1;
                    model.date_created = DateTime.Now;

                    if (model.user_name != "rohasingh" && model.user_name != "csasam" && model.user_name != "flam" && model.user_name != "mkhaled" && model.user_name != "grbell" && model.user_name != "scheduled task")
                    {
                        return Unauthorized();
                    }

                    var jobId = BackgroundJob.Enqueue(() => _serviceManagement.ServiceDatabase(revitModelNames, userName, cancellationToken));

                    return CreatedAtAction("RetrieveJob", new { id = model.job_id }, model);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the AddJob request.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("retrieve/{id}")]
        [ActionName(nameof(RetrieveJob))]
        public IActionResult RetrieveJob(Guid id)
        {
            try
            {
                var model = models.FirstOrDefault(x => x.job_id == id);

                if (model == null)
                {
                    return NotFound();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the GetJob request.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("retrieve")]
        [ActionName(nameof(RetrieveAllJobs))]
        public IActionResult RetrieveAllJobs()
        {
            try
            {
                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the GetAllJobs request.");
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        [ActionName(nameof(DeleteJob))]
        public IActionResult DeleteJob(Guid id)
        {
            try
            {
                var model = models.FirstOrDefault(x => x.job_id == id);

                if (model == null)
                {
                    return NotFound();
                }
                model.status = 0;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the DeleteJob request.");
                return BadRequest();
            }
        }
    }
}
