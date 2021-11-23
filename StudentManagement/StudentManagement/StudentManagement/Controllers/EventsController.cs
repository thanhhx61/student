using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Hubs;
using StudentManagement.Models.Events;
using StudentManagement.Models.Students;
using StudentManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class EventsController : Controller
    {
        private static User user = new User();
        public static int? evtId;
        private readonly UserService userService;
        private readonly EventService eventService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly StudentManagementContext context;
        private readonly IHubContext<ChatHub> _hubContext;

        public EventsController(UserService userService,
                                EventService eventService,
                                IWebHostEnvironment webHostEnvironment,
                                StudentManagementContext context, IHubContext<ChatHub> hubContext)
        {
            this.userService = userService;
            this.eventService = eventService;
            this.webHostEnvironment = webHostEnvironment;
            this.context = context;
            this._hubContext = hubContext;
        }
        [Route("/Events/Index/{userId}")]
        public async Task<IActionResult> Index(string userId, int? SchoolYearId, int? ListEvent, bool checkbox1, bool checkbox2, bool checkbox3, int status1 = 1, int status2 = 2, int status3 = 3)
        {
            var events = from m in context.Events.Include(x=>x.Messages) select m;

            if (!(SchoolYearId == null))
            {
                events = events.Where(s => s.SchoolYearId == SchoolYearId);
            }
            if (!(ListEvent == null))
            {
                events = events.Where(s => s.ListEvent.ListEventId == ListEvent);
            }
            if (checkbox1 && checkbox2 && !checkbox3)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status1 || s.Status == (Enums.EventStatus?)status2);
            }
            else if (checkbox1 && !checkbox2 && !checkbox3)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status1);
            }
            else if (!checkbox1 && checkbox2 && !checkbox3)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status2);
            }
            else if (checkbox1 && checkbox3 && !checkbox2)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status1 || s.Status == (Enums.EventStatus?)status3);
            }
            else if (checkbox1 && !checkbox3 && !checkbox2)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status1);
            }
            else if (!checkbox1 && checkbox3 && !checkbox2)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status3);
            }
            else if (checkbox2 && checkbox3 && !checkbox1)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status2 || s.Status == (Enums.EventStatus?)status3);
            }
            else if (checkbox2 && !checkbox3 && !checkbox1)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status2);
            }
            else if (!checkbox2 && checkbox3 && !checkbox1)
            {
                events = events.Where(s => s.Status == (Enums.EventStatus?)status3);
            }
            user = userService.Get(userId);
            var eventSearch = new SearchEvent
            {
                events = await events.Where(p => p.UserId == userId).OrderByDescending(c => c.SchoolYearId).OrderByDescending(c => c.EventId).ToListAsync()
            };
            var schoolYear = context.UserSchoolYears.Include(u => u.SchoolYear).Include(u => u.User).OrderByDescending(u => u.SchoolYearId).FirstOrDefault(m => m.UserId == userId).SchoolYear.SchoolYearName;
            ViewBag.ListSchoolYearId = await context.SchoolYears.ToListAsync();
            ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
            ViewData["SchoolYearId"] = new SelectList(context.SchoolYears, "SchoolYearId", "SchoolYearName");
            ViewBag.User = user;
            ViewBag.SchoolYear = schoolYear;
            return View(eventSearch);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateEvent create)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    create.UserId = user.Id;
                    var schoolYear = context.UserSchoolYears.Include(u => u.SchoolYear).Include(u => u.User).OrderByDescending(u => u.SchoolYearId).FirstOrDefault(m => m.UserId == user.Id).SchoolYear.SchoolYearId;
                    var events = new Event()
                    {
                        UserId = create.UserId,
                        Status = (Enums.EventStatus?)1,
                        Act = create.Act,
                        Activities = create.Activities,
                        PowerDev = create.PowerDev,
                        PowerExerted = create.PowerExerted,
                        SchoolYearId = schoolYear,
                        Think = create.Think,
                        ListEventId = create.ListEventId
                    };
                    if (eventService.Create(events))
                    {
                        return RedirectToAction("Index", "Events", new { userId = user.Id });
                    }
                }
                ViewBag.User = user;
                ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
                return View(create);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("/Events/Edit/{eventId}")]
        [Authorize]
        public IActionResult Edit(int eventId)
        {
            var events = eventService.Get(eventId);
            if (events == null) return View();
            var edituser = new EditEvents()
            {
                EventId = events.EventId,
                Act = events.Act,
                Activities = events.Activities,
                ListEvent = events.ListEvent,
                PowerDev = events.PowerDev,
                PowerExerted = events.PowerExerted,
                Status = events.Status,
                Think = events.Think,
                UserId = events?.UserId,
                ListEventId = events.ListEventId
            };
            ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
            ViewBag.User = user;
            return View(edituser);
        }
        [HttpPost]
        public IActionResult Edit(EditEvents model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var events = eventService.Get(model.EventId);
                    events.UserId = model.UserId;
                    events.PowerDev = model.PowerDev;
                    events.PowerExerted = model.PowerExerted;
                    events.Think = model.Think;
                    events.ListEvent = model.ListEvent;
                    events.Act = model.Act;
                    events.ListEventId = model.ListEventId;
                    events.Activities = model.Activities;
                    if (eventService.Edit(events))
                    {
                        return RedirectToAction("Index", "Events", new { userId = model.UserId });
                    }
                }
                ViewBag.User = user;
                ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("Events/Delete/{eventId}")]
        public IActionResult Remove(int eventId)
        {
            if (eventService.Remove(eventId))
            {
                return RedirectToAction("Index", "Events", new { userId = user.Id });
            }
            return RedirectToAction("Index", "Detail", new { userId = user.Id });
        }
        [HttpGet]
        [Route("/Events/ChangeStatus23/{eventId}")]
        public IActionResult ChangeStatus23(int eventId)
        {
            ViewBag.EmployeeId = eventId;
            if (eventService.ChangeStatus23(eventId))
            {
                return RedirectToAction("Index", "Events", new { userId = user.Id });
            }
            return RedirectToAction("Index", "Events", new { userId = user.Id });
        }
        [HttpGet]
        [Route("/Events/ChangeStatus12/{eventId}")]
        public IActionResult ChangeStatus12(int eventId)
        {
            ViewBag.EmployeeId = eventId;
            if (eventService.ChangeStatus12(eventId))
            {
                return RedirectToAction("Index", "Events", new { userId = user.Id });
            }
            return RedirectToAction("Index", "Events", new { userId = user.Id });
        }
        [Route("Events/SendMessage/{eventId}")]
        public async Task<IActionResult> SendMessage(int eventId)
        {
            try
            {
                var userName = User.Identity.Name;
                var data = await eventService.GetMessageAsync(eventId, userName);
                data.ForEach((item) => {
                    item.IsMe = item.UserName == userName;
                });
                await _hubContext.Clients.All.SendAsync("ListAllMessageByEventId", data);
                return View();
            }
            catch (Exception e)
            {
            }
            return View();
        }

        [Route("Events/GetAllMessageByEventId/{eventId}")]
        public async Task<IActionResult> GetAllMessageByEventId(int eventId)
        {
            var userName = User.Identity.Name;
            var data = await eventService.GetMessageAsync(eventId, userName);
            data.ForEach((item) => {
                item.IsMe = item.UserName == userName;
            });
            return Ok(data);
        }

    }
}
