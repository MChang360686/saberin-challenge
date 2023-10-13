# saberin-challenge
Changes made to Saberin Intern challenge in C#

# Contact.cs
Added new String quality Primary Email.  Defaults to first email in EmailAddresses if no email of type "Primary" is found

# ContactsController.cs
Added Logger as well as error handling via try-catch blocks.  (Unfinished) Try to recieve JSON to change EmailList

# ControllerLogging.cs
New file, uses ILoggerFactory to make creating loggers in the Controllers directory easier

# EmailType.cs
Added new type to Enum, "Primary"

# HomeController.cs
Added Logger as well as error handling via try-catch blocks

# _ContactTable.cshtml
Edited to show the Primary Email Address in the grid

# _EditContact.cshtml
Edited to add a dropdown for the Title input as well as a jQuery datepicker and necessary script for the DOB input.  Additionally, edited to allow for setting emails as type "Primary" in the email type dropdown.  Added function to check whether or not Email or Address information was not added before hitting save, throwing an alert if this was the case.

