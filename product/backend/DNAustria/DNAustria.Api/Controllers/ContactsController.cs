using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
public class ContactsController
{
    private readonly IContactsLogic _contactsLogic;
    
    public ContactsController(IContactsLogic contactsLogic)
    {
        _contactsLogic = contactsLogic;
    }
    
    [HttpGet]
    [Route("[controller]")]
    public IActionResult Get()
    
}