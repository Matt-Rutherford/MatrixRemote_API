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
        public RemoteAPIController(ILogging logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<MessageDTO>> GetMessages()
        {
            _logger.Log("Getting all messages", "");
            return Ok(RemoteStore.remoteList);
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
            var message = RemoteStore.remoteList.FirstOrDefault(u => u.Id == id);
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
            if (RemoteStore.remoteList.FirstOrDefault(u => u.Message.ToLower() == messageDTO.Message.ToLower()) != null)
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
            messageDTO.Id = RemoteStore.remoteList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            RemoteStore.remoteList.Add(messageDTO);

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
            var message = RemoteStore.remoteList.FirstOrDefault(u => u.Id == id);
            if (message == null)
            {
                return NotFound();
            }
            RemoteStore.remoteList.Remove(message);
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
            var message = RemoteStore.remoteList.FirstOrDefault(u => u.Id == id);
            message.Message = messageDTO.Message;
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
            var message = RemoteStore.remoteList.FirstOrDefault(u => u.Id == id);
            if (message == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(message, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }

}
