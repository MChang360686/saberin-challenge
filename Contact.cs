using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Data
{
    public class Contact : Entity
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public virtual List<EmailAddress> EmailAddresses { get; set; } = new List<EmailAddress>();
        public virtual List<Address> Addresses { get; set; } = new List<Address>();

        public string PrimaryEmail 
        {
            get 
            {
                // Check for email of type Primary
                // Otherwise, return first email in List
                foreach (EmailAddress email in EmailAddresses)
                {
                    if (email.Type == EmailType.Primary)
                    {
                        return email.Email;
                    }
                    else
                    {
                        return EmailAddresses[0].Email;
                    }
                }
                return "No Emails available"; 
            } 

         
        }
    }

}
