using Microsoft.EntityFrameworkCore;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Models;
using StudentManagement.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Services
{
    public class EventService : IEventService
    {
        private readonly StudentManagementContext context;

        public EventService(StudentManagementContext context)
        {
            this.context = context;
        }

        public bool Create(Event model)
        {
            try
            {
                context.Add(model);
                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Edit(Event model)
        {
            try
            {
                context.Attach(model);
                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<Event> GetAsync(int eventId)
        {
            return await context.Events.Include(p => p.User).Include(z => z.Messages).FirstOrDefaultAsync(x => x.EventId == eventId);
        }
        public Event Get(int eventId)
        {
            return context.Events.Include(p => p.User).Include(z => z.Messages).FirstOrDefault(x => x.EventId == eventId);
        }

        public List<Event> GetEventbyUserId(string stuId)
        {
            return context.Events.Include(p => p.User).Include(p => p.ListEvent).Where(p => p.UserId == stuId).OrderByDescending(m => m.EventId).ToList();
        }

        public bool Remove(int id)
        {
            try
            {
                var evt = Get(id);
                context.Remove(evt);
                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ChangeStatus23(int evtId)
        {
            try
            {
                var events = Get(evtId);
                if (events.Status == (Enums.EventStatus)2)
                {
                    events.Status = (Enums.EventStatus)3;
                }
                context.Attach(events);
                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ChangeStatus12(int evtId)
        {
            try
            {
                var events = Get(evtId);
                if (events.Status == (Enums.EventStatus)1)
                {
                    events.Status = (Enums.EventStatus)2;
                }
                context.Attach(events);
                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<MessageViewModel>> GetMessageAsync(int eventId, string userName)
        {
            var messages = await context.Messages.Include(e => e.Event).Include(u => u.User).Where(z => z.EventId == eventId).Select(x => new MessageViewModel()
            {
                Content = x.Content,
                EventId = x.EventId.Value,
                From = x.UserId,
                Timestamp = x.Timestamp,
                UserId = x.UserId,
                UserName = x.User.UserName,
                Avatar = x.User.Avatar,
                MessagesId = x.MessagesId
            }).AsTracking().ToListAsync();
            messages.ForEach((item) =>
            {
                var roleId = context.UserRoles.FirstOrDefault(x => x.UserId == item.UserId)?.RoleId;
                item.RoleName = context.Roles.FirstOrDefault(x => x.Id == roleId)?.Name;
                item.RoleId = roleId;
                item.IsMe = userName == item.UserName;
            });
            return messages;
        }

        
    }
}
