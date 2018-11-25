using System.Collections.Generic;
using System.Linq;
using LibRes.App.DbModels;
using LibRes.App.Models;
using Microsoft.AspNetCore.Identity;

namespace LibRes.App.Data
{
    public static class DbSeeder
	{
        public static void CreateSeedData
        (this LibResDbContext context, UserManager<ApplicationUser> userManager)
        {
            if(!context.RoomModels.Any())
            {
                RoomModel sampleRoomModel = new RoomModel()
                {
                    RoomName = "Sample room 1"
                };
                context.AddRange(sampleRoomModel);
            }

          
            if (userManager.FindByEmailAsync("abc@xyz.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    FirstName = "Pesho",
                    LastName = "Peshev",
                    UserName = "abc@xyz.com",
                    Email = "abc@xyz.com",
                    SecretQuestion = "Am I dead?",
                    SecretAnswer = "Looks Like I am!"
                };

                IdentityResult result = userManager.CreateAsync(user, "Tapaka2000!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

            context.SaveChanges();

            if (!context.ReservationModels.Any())
            {
                ReservationModel sampleReseravtion = new ReservationModel()
                {
                    EventName = "Techno party",
                    EventDates = new HashSet<EventOccuranceModel>(),
                    BeginHour = "12:00",
                    EndHour = "14:00",
                    MeetingRoom = context.RoomModels.First(),
                    Department = "Kupon",
                    ReservationOwner = userManager.FindByEmailAsync("abc@xyz.com").Result,
                    WantsMultimedia = false
                };

                sampleReseravtion.EventDates = new HashSet<EventOccuranceModel>(){
                    new EventOccuranceModel(){
                        Reservation = sampleReseravtion,
                        Occurance = System.DateTime.Now
                    }
                };

                

                context.AddRange(sampleReseravtion);
                context.SaveChanges();
            }

        }
    }
}
