using General.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace General.Services.LibrarySeat
{
    public static class LibrarySeatData
    {
        public static object locker = new object();
        public static int SqlUpdateLibrarySeatTime = 30000;        //单位毫秒
        public static int CheckInTime = 15;     //打卡限定时间 单位分钟
        public static int OrderEndTime = 2;     //预定默认时间 单位小时
        public static int ViolationMaxNum = 3;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new GeneralDbContext(serviceProvider.GetRequiredService<DbContextOptions<GeneralDbContext>>()))
            {
                if (context.librarySeats.Any())
                {
                    return;
                }
                context.librarySeats.AddRange(CreateLibrarySeat(1, 10));
                context.librarySeats.AddRange(CreateLibrarySeat(2, 10));
                context.librarySeats.AddRange(CreateLibrarySeat(0, 95));

                context.SaveChanges();

            }
        }

        public static Entities.LibrarySeat[] CreateLibrarySeat(int floor, int count)
        {
            Entities.LibrarySeat[] librarySeats = new Entities.LibrarySeat[count];
            for (int i = 0; i < count; i++)
            {
                Entities.LibrarySeat seat = new Entities.LibrarySeat();
                seat.Id = Guid.NewGuid();
                seat.Floor = floor;
                seat.SeatNumber = floor * 100 + i + 1;
                seat.SeatState = Entities.LibrarySeat.SeatStates.Available;
                librarySeats[i] = seat;
            }

            return librarySeats;
        }

        public static List<Vector2> seatPosition()
        {
            return new List<Vector2>()
            {
                new Vector2(76,184),
                new Vector2(76,236),
                new Vector2(100,184),
                new Vector2(100,236),
                new Vector2(122,184),
                new Vector2(122,236),
                new Vector2(150,184),
                new Vector2(150,236),
                 new Vector2(170,184),
                new Vector2(170,236),
                 new Vector2(197,184),
                new Vector2(197,236),
                 new Vector2(217,184),
                new Vector2(217,236),
                 new Vector2(244,184),
                new Vector2(244,236),
                 new Vector2(265,184),
                new Vector2(265,236),
                 new Vector2(291,184),
                new Vector2(291,236),
                 new Vector2(311,184),
                new Vector2(311,236),
                 new Vector2(338,184),
                new Vector2(338,236),
                 new Vector2(358,184),
                new Vector2(358,236),
                 new Vector2(385,184),
                new Vector2(385,236),
                 new Vector2(406,184),
                new Vector2(406,236),
                 new Vector2(433,184),
                new Vector2(433,236),
                 new Vector2(453,184),
                new Vector2(453,236),
                 new Vector2(479,184),
                new Vector2(479,236),

                new Vector2(75,474),
                new Vector2(75,526),
                new Vector2(103,474),
                new Vector2(103,526),
                new Vector2(122,474),
                new Vector2(122,526),
                new Vector2(150,474),
                new Vector2(150,526),
                new Vector2(170,474),
                new Vector2(170,526),
                new Vector2(197,474),
                new Vector2(197,526),
                new Vector2(217,474),
                new Vector2(217,526),
                new Vector2(244,474),
                new Vector2(244,526),
                new Vector2(265,474),
                new Vector2(265,526),
                new Vector2(291,474),
                new Vector2(291,526),
                new Vector2(311,474),
                new Vector2(311,526),
                new Vector2(338,474),
                new Vector2(338,526),





            };

        }
    }
}
