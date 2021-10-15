using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BELT.Models
{
    public class Shindig
    {
    [Key]
        public int ShindigId { get; set; }

	[Required]
	[MinLength(2, ErrorMessage="Title must be at least 2 characters")]
        public string Title { get; set; }


    [Required]
    [PastDateAttribute]
        public DateTime Date { get; set; }


    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        public int Duration {get;set;}

    [Required]
    public string DurationType {get;set;}

	[Required]
        public string Description { get; set; }

    public DateTime EndTime {get;set;}

        public List<RSVP> RSVPs {get;set;}

        public int UserId {get;set;}
        public User User {get;set;}



        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }




    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is DateTime)
            {
                DateTime checkMe = (DateTime)value;
                if(checkMe < DateTime.Now)
                {
                    return new ValidationResult("That's the Past!");                        
                }
                else 
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("Not a date");
            }
        }
    }
}