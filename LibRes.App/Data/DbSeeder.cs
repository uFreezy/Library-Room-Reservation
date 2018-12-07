using System;
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
            if (!context.RoomModels.Any())
            {
                var sampleRoomModel = new RoomModel
                {
                    RoomName = "Sample room 1"
                };
                var sampleRoomModel2 = new RoomModel
                {
                    RoomName = "Sample room 2"
                };
                context.AddRange(sampleRoomModel);
                context.AddRange(sampleRoomModel2);
            }


            if (userManager.FindByEmailAsync("abc@xyz.com").Result == null)
            {
                var user = new ApplicationUser
                {
                    FirstName = "Pesho",
                    LastName = "Peshev",
                    UserName = "abc@xyz.com",
                    Email = "abc@xyz.com",
                    SecretQuestion = "Am I dead?",
                    SecretAnswer = "Looks Like I am!"
                };

                var result = userManager.CreateAsync(user, "Tapaka2000!").Result;

                if (result.Succeeded) userManager.AddToRoleAsync(user, "Admin").Wait();
            }

            context.SaveChanges();

            if (!context.ReservationModels.Any())
            {
                var sampleReservation = new ReservationModel
                {
                    EventName = "Techno party",
                    EventDates = new HashSet<EventOccurenceModel>(),
                    MeetingRoom = context.RoomModels.First(),
                    Department = "Kupon",
                    ReservationOwner = userManager.FindByEmailAsync("abc@xyz.com").Result,
                    WantsMultimedia = false
                };

                sampleReservation.EventDates = new HashSet<EventOccurenceModel>
                {
                    new EventOccurenceModel
                    {
                        Reservation = sampleReservation,
                        Occurence = DateTime.Now,
                        DurationMinutes = 60.0
                    }
                };


                context.AddRange(sampleReservation);
                context.SaveChanges();
            }
        }
    }
}