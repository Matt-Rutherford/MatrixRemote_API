using MatrixRemote_RemoteAPI.Data;
using MatrixRemote_RemoteAPI.Models;
using MatrixRemote_RemoteAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MatrixRemote_RemoteAPI.Controllers
{
    [Route("api/RemoteAPI")]
    [ApiController]
    public class RemoteAPIController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<MessageDTO>> GetMessages()
        {
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
            if (RemoteStore.remoteList.FirstOrDefault(u => u.Name.ToLower() == messageDTO.Name.ToLower()) != null)
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

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteMessage")]
        public IActionResult DeleteMessage(int id)
        {
            if(id == 0)
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}", Name = "UpdateMessage")]
        public IActionResult UpdateMessage(int id, [FromBody]MessageDTO messageDTO) 
        {
            if(messageDTO == null || id != messageDTO.Id)
            {
                return BadRequest();
            }
            var message = RemoteStore.remoteList.FirstOrDefault(u => u.Id == id);
            message.Name = messageDTO.Name;
            return NoContent();
        }
    }
    
}
