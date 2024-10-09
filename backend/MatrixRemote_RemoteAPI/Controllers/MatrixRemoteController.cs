using MatrixRemote_RemoteAPI.Data;
using MatrixRemote_RemoteAPI.Logging;
using MatrixRemote_RemoteAPI.Models;
using MatrixRemote_RemoteAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;  // Needed for checking if in dev/prod
using System.Drawing;

// These controllers are for display messages on API. #TODO move MessageDTO functions? 
namespace MatrixRemote_RemoteAPI.Controllers
{
    [Authorize] // To make 
    [Route("api/RemoteAPI")]
    [ApiController]
    public class RemoteAPIController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly AppDbContext _db;
        private readonly IHostEnvironment _env; 

        public RemoteAPIController(AppDbContext db, ILogging logger, IHostEnvironment env)
        {
            _db = db;
            _logger = logger;
            _env = env;
        }


        [HttpGet]
        //[Produces("application/json")] // Explicitly produce JSON
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<MessageDTO>> GetMessages()
        {
            _logger.Log("Getting all messages", "");
            var messages = _db.Remotes
                      .Select(remote => new MessageDTO
                      {
                          Id = remote.Id,
                          Message = remote.Message,
                          Font = remote.Font,
                          ImageUrl = remote.ImageUrl
                          // Add other properties if needed
                      })
                      .ToList();

            // Log the retrieved messages count (or details if necessary)
            _logger.LogInformation("Retrieved {Count} messages", messages.Count);
            return Ok(messages); //Retrieves all the villas, include .ToList() to convert to list? 
        }

        [HttpGet("{id:int}", Name = "GetMessage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type = typeof(RemoteDTO)] // Alternate to above
        public ActionResult<MessageDTO> GetMessage(int id)
        {

            if (id == 0)
            {
                _logger.Log("Get Message Error With Id" + id, "error");
                return BadRequest();
            }
            var message = _db.Remotes.FirstOrDefault(u => u.Id == id);
            if (message == null)
            {
                return NotFound();
            }
            return Ok(message);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<MessageDTO> CreateMessage([FromBody] MessageDTO messageDTO)
        {
            if (_db.Remotes.FirstOrDefault(u => u.Message.ToLower() == messageDTO.Message.ToLower()) != null)
            {
                ModelState.AddModelError("ExistingMessageError", "Message already Exists!");
                return BadRequest(ModelState);
            }
            if (messageDTO == null)
            {
                return BadRequest();
            }
            if (messageDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Remote model = new()
            {
                Id = messageDTO.Id,
                Font = messageDTO.Font,
                ImageUrl = messageDTO.ImageUrl,
                Message = messageDTO.Message
            };

            _db.Remotes.Add(model);
            _db.SaveChanges();


            return CreatedAtRoute("GetMessage", new { id = messageDTO.Id }, messageDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteMessage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteMessage(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var message = _db.Remotes.FirstOrDefault(u => u.Id == id);
            if (message == null)
            {
                return NotFound();
            }
            _db.Remotes.Remove(message);
            _db.SaveChanges();
            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateMessage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateMessage(int id, [FromBody] MessageDTO messageDTO)
        {
            if (messageDTO == null || id != messageDTO.Id)
            {
                return BadRequest();
            }
            Remote model = new()
            {
                Id = messageDTO.Id,
                Font = messageDTO.Font,
                ImageUrl = messageDTO.ImageUrl,
                Message = messageDTO.Message
            };
            _db.Remotes.Update(model);
            _db.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id:int}", Name = "UpdatePartialMessage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialMessage(int id, JsonPatchDocument<MessageDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var message = _db.Remotes.AsNoTracking().FirstOrDefault(u => u.Id == id);

            MessageDTO messageDTO = new()
            {
                Id = message.Id,
                Font = message.Font,
                ImageUrl = message.ImageUrl,
                Message = message.Message
            };

            if (message == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(messageDTO, ModelState);

            Remote model = new()
            {
                Id = messageDTO.Id,
                Font = messageDTO.Font,
                ImageUrl = messageDTO.ImageUrl,
                Message = messageDTO.Message
            };

            _db.Remotes.Update(model);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

        //method for testing display message. I don't like the logic for checking if env is in prod or dev #TODO refactor? 
        [AllowAnonymous] //TODO REMOVE
        [HttpPost("DisplayMessage")]
        public IActionResult DisplayMessage([FromBody] MessageInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Message) || input.Color == null)
            {
                return BadRequest("Message and Color are required.");
            }
            
            try
            {
                if (_env.IsProduction())
                {
                    // Construct the Linux command using the input
                    string command = $"sudo ./rpi-rgb-led-matrix/utils/text-scroller -f ./rpi-rgb-led-matrix/fonts/9x18.bdf -C{input.Color.R},{input.Color.G},{input.Color.B} --\r\nled-cols=64 --led-rows=64 \"{input.Message}\"";
                    //string command = $"/path/to/matrix/display -m \"{input.Message}\" -c \"{input.Color}\"";

                    // run the command on the Pi
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash", // Assuming you're running bash shell commands
                        Arguments = $"-c \"{command}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(processInfo))
                    {
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            return Ok("Message displayed successfully.");
                        }
                        else
                        {
                            string error = process.StandardError.ReadToEnd();
                            return StatusCode(500, $"Error: {error}");
                        }
                    }
                } else {
                    return Ok("[Development] Would have displayed message: {input.Message} in R = {input.Color.R}, G = {input.Color.G}, B = {input.Color.B}");
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Exception occurred: {ex.Message}");
            }
        }

    }

}
