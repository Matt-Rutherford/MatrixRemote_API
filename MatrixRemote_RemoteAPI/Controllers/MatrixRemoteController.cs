using MatrixRemote_RemoteAPI.Data;
using MatrixRemote_RemoteAPI.Logging;
using MatrixRemote_RemoteAPI.Models;
using MatrixRemote_RemoteAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MatrixRemote_RemoteAPI.Controllers
{
    [Route("api/RemoteAPI")]
    [ApiController]
    public class RemoteAPIController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly AppDbContext _db;

        public RemoteAPIController(AppDbContext db, ILogging logger)
        {
            _db = db;
            _logger = logger;
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
            var message = _db.Remotes.FirstOrDefault(u => u.Id == id);

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
    }

}
