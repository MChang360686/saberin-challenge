using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Data;
using ContactManager.Hubs;
using ContactManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;

namespace ContactManager.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHubContext<ContactHub> _hubContext;

        ILogger<ContactsController> controllerLogger = ControllerLogging.CreateLogger<ContactsController>();

        public ContactsController(ApplicationContext context, IHubContext<ContactHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> DeleteContact(Guid id)
        {
            var contactToDelete = await _context.Contacts
                .Include(x => x.EmailAddresses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (contactToDelete == null)
            {
                return BadRequest();
            }

            _context.EmailAddresses.RemoveRange(contactToDelete.EmailAddresses);
            _context.Contacts.Remove(contactToDelete);

            try
            {
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("Update");

                return Ok();
            }
            catch (Exception ex)
            {
                controllerLogger.LogError("Delete Contact Error: " + ex.Message);
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
            
        }

        public async Task<IActionResult> EditContact(Guid id)
        {
            var contact = await _context.Contacts
                .Include(x => x.EmailAddresses)
                .Include(x => x.Addresses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            var viewModel = new EditContactViewModel
            {
                Id = contact.Id,
                Title = contact.Title,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                DOB = contact.DOB,
                EmailAddresses = contact.EmailAddresses,
                Addresses = contact.Addresses
            };

            return PartialView("_EditContact", viewModel);
        }

        public async Task<IActionResult> GetContacts()
        {
            var contactList = await _context.Contacts
                .OrderBy(x => x.FirstName)
                .ToListAsync();

            return PartialView("_ContactTable", new ContactViewModel { Contacts = contactList });
        }

        public IActionResult Index()
            {
                return View();
            }

        public IActionResult NewContact()
        {
            return PartialView("_EditContact", new EditContactViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SaveContact([FromBody]SaveContactViewModel model)
        {
            var contact = model.ContactId == Guid.Empty
                ? new Contact { Title = model.Title, FirstName = model.FirstName, LastName = model.LastName, DOB = model.DOB }
                : await _context.Contacts.Include(x => x.EmailAddresses).Include(x => x.Addresses).FirstOrDefaultAsync(x => x.Id == model.ContactId);

            if (contact == null)
            {
                return NotFound();
            }

            _context.EmailAddresses.RemoveRange(contact.EmailAddresses);
            _context.Addresses.RemoveRange(contact.Addresses);


            foreach (var email in model.Emails)
            {
                try
                {
                    contact.EmailAddresses.Add(new EmailAddress
                    {
                        Type = email.Type,
                        Email = email.Email,
                        Contact = contact
                    });
                }
                catch (Exception ex)
                {
                    controllerLogger.LogError("Error adding Email Address" + ex.Message);
                }
            }

            foreach (var address in model.Addresses)
            {
                try
                {
                    contact.Addresses.Add(new Address
                    {
                        Street1 = address.Street1,
                        Street2 = address.Street2,
                        City = address.City,
                        State = address.State,
                        Zip = address.Zip,
                        Type = address.Type
                    });
                }
                catch (Exception ex)
                {
                    controllerLogger.LogError("Error adding Address" + ex.Message);
                }
            }

            contact.Title = model.Title;
            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.DOB = model.DOB;

            if (model.ContactId == Guid.Empty)
            {
                await _context.Contacts.AddAsync(contact);
            }
            else
            {
                _context.Contacts.Update(contact);
            }


            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("Update");

            return Ok();
        }

        // Experimental disregard for now
        [HttpPost]
        public JsonResult AddEmail(string email, string type)
        {
            try
            {

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                controllerLogger.LogError("Add Contact Email Error: " + ex.Message);
                return Json(new { success = false });
            }
        }

    }

}