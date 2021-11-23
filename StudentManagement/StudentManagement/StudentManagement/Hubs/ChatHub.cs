using Microsoft.AspNetCore.SignalR;
using StudentManagement.Entities;
using StudentManagement.Models;
using StudentManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StudentManagement.Hubs
{
    public class ChatHub : Hub
    {
        public readonly static List<UserViewModel> listUsers = new List<UserViewModel>();

        private readonly static Dictionary<string, string> listUsersMap = new Dictionary<string, string>();
        private UserService _userService;
        private EventService _eventService;
        private MessageService _messageService;
        public ChatHub(UserService userService, EventService eventService, MessageService messageService)
        {
            _userService = userService;
            _eventService = eventService;
            _messageService = messageService;
        }
        private string IdentityName
        {
            get { return Context.User.Identity.Name; }
        }

        public async Task SendMessage(string eventId, string message)
        {
            //var userId = Context.User.
            try
            {
                var connectId = Context.ConnectionId;
                var user = await _userService.GetUserByUserName(IdentityName);
                var messageCreate = new Message()
                {
                    Content = message,
                    EventId = Convert.ToInt32(eventId),
                    UserId = user?.Id,
                    Timestamp = DateTime.Now,
                };
                var timestamp = messageCreate.Timestamp != null ? messageCreate.Timestamp.Value.ToString("hh:mm tt dd/MM/yyyy") : "";
                var isMe = false;
                var userNameConnect = listUsersMap.FirstOrDefault(x => x.Value == connectId).Key;
                isMe = userNameConnect == IdentityName;


                var messageId = _messageService.Create(messageCreate);
                var messageResult = new MessageViewModel()
                {
                    Content = messageCreate.Content,
                    EventId = messageCreate.EventId,
                    From = messageCreate.UserId,
                    Timestamp = messageCreate.Timestamp,
                    UserId = messageCreate.UserId,
                    UserName = user.UserName,
                    Avatar = user.Avatar,
                    MessagesId = messageId
                };
                //await Clients.Client(userNameConnect).SendAsync("ReceiveMessage", IdentityName, message, timestamp, isMe);

                //await Clients.Client(IdentityName).SendAsync("ReceiveMessage", IdentityName, message, timestamp, isMe);
                //await Clients.Caller.SendAsync("ReceiveMessage", IdentityName, message, timestamp, isMe);
                await Clients.All.SendAsync("ReceiveMessage", messageResult);
            }
            catch (Exception ex)
            {
            }
        }


        public async Task GetAllMessageByEventId(string eventId)
        {
            try
            {
                var idEvent = Convert.ToInt32(eventId);
                var messages = await _eventService.GetMessageAsync(idEvent, Context.User.Identity.Name);
                await Clients.All.SendAsync("ListAllMessageByEventId", messages);
            }
            catch (Exception e)
            {
            }
        }
        public async Task DeleteMessageById(string messageId)
        {
            try
            {
                var idMessage = Convert.ToInt32(messageId);
                await _messageService.DeleteMessageById(idMessage);
            }
            catch (Exception e)
            {
            }
        }


        public override Task OnConnectedAsync()
        {
            try
            {
                var user = _userService.Gets().Where(u => u.UserName == IdentityName).Select(x => new UserViewModel()
                {
                    UserName = x.UserName,
                    FullName = x.UserName,
                    Avatar = x.Avatar
                }).FirstOrDefault();

                if (!listUsers.Any(u => u.UserName == IdentityName))
                {
                    listUsers.Add(user);
                    listUsersMap.Add(IdentityName, Context.ConnectionId);
                }

                Clients.Caller.SendAsync("getProfileInfo", user);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var user = listUsers.Where(u => u.UserName == IdentityName).First();
                listUsers.Remove(user);

                // Tell other users to remove you from their list
                Clients.OthersInGroup(user.EventId.ToString()).SendAsync("removeUser", user);

                // Remove mapping
                listUsersMap.Remove(user.UserName);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }

            return base.OnDisconnectedAsync(exception);
        }


    }
}
