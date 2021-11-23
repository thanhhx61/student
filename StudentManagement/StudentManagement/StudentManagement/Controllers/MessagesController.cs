using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudentManagement.Hubs;
using StudentManagement.Models;
using StudentManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class MessagesController : Controller
    {

        private MessageService _messageService;
        private IHubContext<ChatHub> _hubContext;
        public MessagesController(MessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }
        [Route("/messages/{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _messageService.DeleteMessageById(id);

            await _hubContext.Clients.All.SendAsync("removeMessage", id);
            await _hubContext.Clients.All.SendAsync("onMessageDeleted", string.Format("Room {0} has been deleted.\nYou are moved to the first available room!", id));

            return NoContent();
        }

        [Route("/messages/{id}")]
        [HttpPut]
        public async Task<IActionResult> Edit(int id, MessageUpdateViewModel message)
        {
            _messageService.Edit(message);

            await _hubContext.Clients.All.SendAsync("updateMessage", new { id = message.MessageId, message.Content });
            //await _hubContext.Clients.All.SendAsync("onUpdateMessage", new { id = message.MessageId, message.Content });

            return NoContent();
        }

    }
}
