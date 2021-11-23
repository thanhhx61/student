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
    public class MessageService
    {
        private readonly StudentManagementContext _context;

        public MessageService(StudentManagementContext context)
        {
            _context = context;
        }

        public int Create(Message model)
        {
            try
            {
                _context.Add(model);
                _context.SaveChanges();
                return model.MessagesId;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public bool Edit(MessageUpdateViewModel model)
        {
            try
            {
                var mes = Get(model.MessageId);
                mes.Content = model.Content ?? "";
                _context.Entry(mes).State = EntityState.Detached;
                _context.Entry(mes).State = EntityState.Modified;
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Message Get(int messagesId)
        {
            return _context.Messages.FirstOrDefault(x => x.MessagesId == messagesId);
        }

        public List<Event> GetEventbyUserId(string stuId)
        {
            return _context.Events.Include(p => p.User).Include(p => p.ListEvent).Where(p => p.UserId == stuId).OrderByDescending(m => m.EventId).ToList();
        }


        public async Task<List<Message>> GetMessageAsync(int eventId)
        {
            return await _context.Messages.Where(x => x.EventId == eventId).ToListAsync();
        }

        public async Task DeleteMessageById(int messageId)
        {
            try
            {
                var evt = _context.Messages.FirstOrDefault(x => x.MessagesId == messageId);
                _context.Remove(evt);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
