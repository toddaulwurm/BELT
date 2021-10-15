using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BELT.Models
{
    public class RSVP
    {
    [Key]
        public int RSVPId { get; set; }


        public int UserId {get;set;}
        public User User {get;set;}

        public int ShindigId {get;set;}
        public Shindig Shindig {get;set;}


    }
}