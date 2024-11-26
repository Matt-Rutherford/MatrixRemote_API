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
using static MatrixRemote_RemoteAPI.Models.MatrixEvent;

// These controllers are for display messages on API. #TODO move MessageDTO functions? 
namespace MatrixRemote_RemoteAPI.Controllers
{
    [Authorize]
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

        #region Helper Methods

        private MatrixEventDTO MapToDTO(MatrixEvent eventEntity)
        {
            return new MatrixEventDTO
            {
                Id = eventEntity.Id,
                Content = eventEntity.Content,
                Timestamp = eventEntity.Timestamp,
                Location = eventEntity.Location == null ? null : new GeoLocationDTO
                {
                    Latitude = eventEntity.Location.Latitude,
                    Longitude = eventEntity.Location.Longitude
                },
                Type = (EventTypeDTO)eventEntity.Type,
                Color = eventEntity.Color == null ? null : new RgbColorDTO
                {
                    R = eventEntity.Color.R,
                    G = eventEntity.Color.G,
                    B = eventEntity.Color.B
                }
            };
        }

        private MatrixEvent MapToEntity(MatrixEventDTO eventDTO)
        {
            return new MatrixEvent
            {
                Id = eventDTO.Id == Guid.Empty ? Guid.NewGuid() : eventDTO.Id,
                Content = eventDTO.Content,
                Timestamp = eventDTO.Timestamp == default ? DateTime.UtcNow : eventDTO.Timestamp,
                Location = eventDTO.Location == null ? null : new GeoLocation
                {
                    Latitude = eventDTO.Location.Latitude,
                    Longitude = eventDTO.Location.Longitude
                },
                Type = (EventType)eventDTO.Type,
                Color = eventDTO.Color == null ? null : new RgbColor
                {
                    R = eventDTO.Color.R,
                    G = eventDTO.Color.G,
                    B = eventDTO.Color.B
                }
            };
        }

        private ActionResult? ValidateEventDTO(MatrixEventDTO eventDTO)
        {
            if (eventDTO == null || string.IsNullOrWhiteSpace(eventDTO.Content))
                return BadRequest("Event content is required.");

            if (!Enum.IsDefined(typeof(EventTypeDTO), eventDTO.Type))
                return BadRequest($"Invalid event type: {eventDTO.Type}");

            if (eventDTO.Type == EventTypeDTO.Text && eventDTO.Color == null)
                return BadRequest("RGB Color is required for Text events.");

            if ((eventDTO.Type == EventTypeDTO.Image || eventDTO.Type == EventTypeDTO.GIF) && eventDTO.Color != null)
                return BadRequest("RGB Color is not applicable for Image or GIF events.");

            return null;
        }


        #endregion

        #region Endpoints

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<MatrixEventDTO>> GetEvents()
        {
            _logger.Log("Getting all events", "");

            var events = _db.Events.Select(MapToDTO).ToList();

            _logger.LogInformation("Retrieved {Count} events", events.Count);

            return Ok(events);
        }

        [HttpGet("{id:guid}", Name = "GetEvent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MatrixEventDTO> GetEvent(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid ID.");

            var eventEntity = _db.Events.FirstOrDefault(u => u.Id == id);
            if (eventEntity == null)
                return NotFound($"No event found with ID: {id}");

            return Ok(MapToDTO(eventEntity));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MatrixEventDTO> CreateEvent([FromBody] MatrixEventDTO eventDTO)
        {
            var validationResult = ValidateEventDTO(eventDTO);
            if (validationResult != null)
                return validationResult;

            var newEvent = MapToEntity(eventDTO);

            _db.Events.Add(newEvent);
            _db.SaveChanges();

            return CreatedAtRoute("GetEvent", new { id = newEvent.Id }, MapToDTO(newEvent));
        }


        [HttpPut("{id:guid}", Name = "UpdateEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateEvent(Guid id, [FromBody] MatrixEventDTO eventDTO)
        {
            if (id != eventDTO.Id)
                return BadRequest("Mismatched ID.");

            var validationResult = ValidateEventDTO(eventDTO);
            if (validationResult != null)
                return validationResult;

            var updatedEvent = MapToEntity(eventDTO);

            _db.Events.Update(updatedEvent);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id:guid}", Name = "DeleteEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteEvent(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid ID.");

            var eventEntity = _db.Events.FirstOrDefault(u => u.Id == id);
            if (eventEntity == null)
                return NotFound();

            _db.Events.Remove(eventEntity);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:guid}", Name = "UpdatePartialEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialEvent(Guid id, JsonPatchDocument<MatrixEventDTO> patchDTO)
        {
            if (patchDTO == null || id == Guid.Empty)
                return BadRequest();

            var eventEntity = _db.Events.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (eventEntity == null)
                return NotFound($"Event with ID {id} not found.");

            var eventDTO = MapToDTO(eventEntity);

            patchDTO.ApplyTo(eventDTO, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedEvent = MapToEntity(eventDTO);

            _db.Events.Update(updatedEvent);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPost("DisplayEvent")]
        public IActionResult DisplayEvent([FromBody] MatrixEvent input)
        {
            if (string.IsNullOrWhiteSpace(input.Content))
                return BadRequest("Event content is required.");

            try
            {
                if (_env.IsProduction())
                {
                    string command = input.Type switch
                    {
                        EventType.Text when input.Color != null =>
                            $"./text-scroller -f ../fonts/9x18.bdf -C{input.Color.R},{input.Color.G},{input.Color.B} --led-cols=64 --led-rows=64 \"{input.Content}\"",
                        _ => throw new NotSupportedException("Unsupported event type.")
                    };

                    ProcessStartInfo processInfo = new()
                    {
                        FileName = "/usr/bin/sudo",
                        Arguments = command,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = "/home/matt/rpi-rgb-led-matrix/utils"
                    };

                    using (Process process = Process.Start(processInfo))
                    {
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                            return Ok("Event displayed successfully.");
                        else
                            return StatusCode(500, process.StandardError.ReadToEnd());
                    }
                }
                else
                {
                    return Ok($"[Development] Would have displayed event: {input.Content} as {input.Type}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Exception occurred: {ex.Message}");
            }
        }

        #endregion
    }
}
