using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models
{
    public class ApplicationUser:IdentityUser
    {
        
        public string? Name { get; set; }
    
     
    }
}
