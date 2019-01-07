using System;
using System.Collections.Generic;
using System.Linq;
using LibRes.App.Controllers;
using LibRes.App.DbModels;
using LibRes.App.Models.Calendar;
using LibRes.App.Models.Enums;
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
                    RoomName = "West Meeting Hall"
                };
                var sampleRoomModel2 = new RoomModel
                {
                    RoomName = "East Meeting Hall"
                };
                context.AddRange(sampleRoomModel);
                context.AddRange(sampleRoomModel2);
            }


            if (userManager.FindByEmailAsync("info@nbu.bg").Result == null)
            {
                var user = new ApplicationUser
                {
                    FirstName = "Pesho",
                    LastName = "Peshev",
                    UserName = "info@nbu.bg",
                    Email = "info@nbu.bg",
                    SecretQuestion = "Am I dead?",
                    SecretAnswer = "Looks Like I am!"
                };

                var result = userManager.CreateAsync(user, "Tapaka2000!").Result;

                if (result.Succeeded) userManager.AddToRoleAsync(user, "Admin").Wait();
            }

            if (userManager.FindByEmailAsync("test@nbu.bg").Result == null)
            {
                var user = new ApplicationUser
                {
                    FirstName = "Ivan",
                    LastName = "Petrov",
                    UserName = "test@nbu.bg",
                    Email = "test@nbu.bg",
                    SecretQuestion = "Question",
                    SecretAnswer = "Answer"
                };

                var result = userManager.CreateAsync(user, "Tapaka2000!").Result;

                if (result.Succeeded) userManager.AddToRoleAsync(user, "Admin").Wait();
            }

            context.SaveChanges();

            if (!context.ReservationModels.Any())
            {
                var sampleReservation = new ReservationModel
                {
                    EventName = "Techno Party",
                    EventDates = new HashSet<EventOccurenceModel>(),
                    MeetingRoom = context.RoomModels.First(),
                    Department = "Party",
                    Description = "It's a huge party",
                    ReservationOwner = userManager.FindByEmailAsync("test@nbu.bg").Result,
                    WantsMultimedia = false
                };

                var sampleCem = new CreateEventModel
                {
                    BeginHour = new DateTime().AddHours(12),
                    Department = "",
                    Description = "",
                    EndHour = new DateTime().AddHours(13),
                    EventDate = DateTime.Now.AddDays(-30),
                    EventName = "",
                    EventRepeatModel = new EventRepeatViewModel
                    {
                        DaysOfTheWeek = new List<DaysOfWeekEnumModel>
                        {
                            new DaysOfWeekEnumModel
                            {
                                DaysOfWeek = DayOfWeek.Monday,
                                IsSelected = true
                            },
                            new DaysOfWeekEnumModel
                            {
                                DaysOfWeek = DayOfWeek.Friday,
                                IsSelected = true
                            }
                        },
                        ExitDate = DateTime.Now.AddYears(2),
                        ExitStrategy = ExitStrategy.NEVER,
                        RepeatInterval = 1,
                        RepeatOption = EventRepeatOptions.Weekly
                    },
                    IsReoccuring = true,
                    MeetingRoomId = "1",
                    WantsMultimedia = false
                };


                CalendarController.SetDateOccurrences(sampleCem, sampleReservation);

                var sampleReservation2 = new ReservationModel
                {
                    EventName = "Very Serious Event",
                    EventDates = new HashSet<EventOccurenceModel>(),
                    MeetingRoom = context.RoomModels.First(),
                    Department = "Something Very Important",
                    Description = "You should go study now!",
                    ReservationOwner = userManager.FindByEmailAsync("test@nbu.bg").Result,
                    WantsMultimedia = false
                };

                var sampleCem2 = new CreateEventModel
                {
                    BeginHour = new DateTime().AddHours(7),
                    Department = "",
                    Description = "",
                    EndHour = new DateTime().AddHours(9),
                    EventDate = DateTime.Now.AddDays(-10),
                    EventName = "",
                    EventRepeatModel = new EventRepeatViewModel
                    {
                        DaysOfTheWeek = new List<DaysOfWeekEnumModel>
                        {
                            new DaysOfWeekEnumModel
                            {
                                DaysOfWeek = DayOfWeek.Wednesday,
                                IsSelected = true
                            },
                            new DaysOfWeekEnumModel
                            {
                                DaysOfWeek = DayOfWeek.Tuesday,
                                IsSelected = true
                            }
                        },
                        ExitDate = DateTime.Now.AddYears(2),
                        ExitStrategy = ExitStrategy.NEVER,
                        RepeatInterval = 1,
                        RepeatOption = EventRepeatOptions.Monthly
                    },
                    IsReoccuring = true,
                    MeetingRoomId = "1",
                    WantsMultimedia = false
                };


                CalendarController.SetDateOccurrences(sampleCem2, sampleReservation2);


                context.AddRange(new List<ReservationModel> {sampleReservation, sampleReservation2});
                context.SaveChanges();
            }
        }
    }
}