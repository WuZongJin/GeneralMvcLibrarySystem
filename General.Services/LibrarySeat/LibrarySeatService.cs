using General.Core;
using General.Core.Data;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static General.Entities.LibrarySeat;

namespace General.Services.LibrarySeat
{
    public class LibrarySeatService : ILibrarySeatService
    {

        private IRepository<Entities.LibrarySeat> _librarySeatRepository;
        private IRepository<Entities.SysUser> _userRepository;
        private IRepository<Entities.OrderDetail> _orderRepository;


        public LibrarySeatService(IRepository<Entities.LibrarySeat> librarySeatRepository, IRepository<Entities.SysUser> userRepository, IRepository<Entities.OrderDetail> orderRepositiory)
        {
            _librarySeatRepository = librarySeatRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepositiory;


        }



        public IList<Entities.LibrarySeat> GetALLLibrarySeat()
        {
            return _librarySeatRepository.Table.ToList();
        }

        public IList<Entities.LibrarySeat> GetLibrarySetsAvailable(int floor)
        {
            return _librarySeatRepository.Table.Where(s => s.Floor == floor && s.SeatState == SeatStates.Available).ToList();
        }

        public IList<Entities.LibrarySeat> GetLibrarySetsBooked(int floor)
        {
            return _librarySeatRepository.Table.Where(s => s.Floor == floor && s.SeatState == SeatStates.Booked).ToList();
        }

        public IList<Entities.LibrarySeat> GetLibrarySetsInAvailable(int floor)
        {
            return _librarySeatRepository.Table.Where(s => s.Floor == floor && s.SeatState == SeatStates.InAvailable).ToList();
        }

        public (int avilable, int booked, int inavilable) GetLibrarySeatCount(int floor)
        {
            int avilable = _librarySeatRepository.Table.Where(s => s.Floor == floor && s.SeatState == SeatStates.Available).Count();
            int booked = _librarySeatRepository.Table.Where(s => s.Floor == floor && s.SeatState == SeatStates.Booked).Count();
            int inavilable = _librarySeatRepository.Table.Where(s => s.Floor == floor && s.SeatState == SeatStates.InAvailable).Count();

            return (avilable, booked, inavilable);
        }

        public (bool state, string message, Entities.LibrarySeat seat, Entities.OrderDetail order) Schedule(int seatnumber, Guid userId)
        {
            var seat = GetBySeatNum(seatnumber);
            var user = _userRepository.getById(userId);
            if (user.ScheduledLock)
            {
                if(user.AllowScheduleTime < DateTime.Now)
                {
                    user.ScheduledLock = false;
                    user.ViolationNum = 0;
                    user.AllowScheduleTime = null;
                    _userRepository.update(user);
                }
                else
                {
                    return (false, $"你违规次数过多，已禁止你进行预定，解锁时间:{user.AllowScheduleTime}", null, null);
                }
            }

            if(seat == null)
            {
                return (false, "座位不存在", null, null);
            }

            if (seat.SeatState == SeatStates.Booked)
            {
                return (false, "座位已被预定", null, null);
            }
            else if (seat.SeatState == SeatStates.InAvailable)
            {
                return (false, "座位上已经有人了", null, null);
            }

            lock (LibrarySeatData.locker)
            {
                var order = new Entities.OrderDetail();
                order.Id = Guid.NewGuid();
                order.LibrarySeatId = seat.Id;
                order.HasCheckIn = false;
                order.CreateTime = DateTime.Now;
                order.VerificationCode = "558879";
                order.EndTime = DateTime.Now.AddHours(LibrarySeatData.OrderEndTime);
                order.HasEnd = false;

                seat.OrderDetails.Add(order);
                user.OrderDetails.Add(order);

                if (seat.SeatState == SeatStates.Available)
                {
                    seat.SeatState = SeatStates.Booked;
                }
                else
                {
                    return (false, "位置信息以改变", null, null);
                }


                try
                {
                    _librarySeatRepository.DbContext.SaveChanges();

                }
                catch (Exception)
                {
                    return (false, "发生错误请重试！", null, null);
                }

                return (true, "预定成功", seat, order);
            }
        }

        public bool ChageLibrarySeatState(Guid seatId, SeatStates state)
        {
            lock (LibrarySeatData.locker)
            {
                var seat = _librarySeatRepository.getById(seatId);
                if (seat == null)
                    return false;

                seat.SeatState = state;
                try
                {
                    _librarySeatRepository.update(seat);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }
        }

        public void UpdateSeatState()
        {
            var ordersHasNotEnd = _orderRepository.Table
                .Where(o => o.HasEnd == false);

            var orderCheckInTimeOut = ordersHasNotEnd
                .Where(o => o.HasCheckIn == false && DateTime.Now >= o.CreateTime.AddMinutes(LibrarySeatData.CheckInTime));

            var orderEndTimeOut = ordersHasNotEnd
                .Where(o => DateTime.Now >= o.EndTime);

            var shouldChangeStateorder = orderEndTimeOut.Union(orderCheckInTimeOut);

            foreach (var order in shouldChangeStateorder)
            {
                var seat = _librarySeatRepository.getById(order.LibrarySeatId);
                seat.SeatState = SeatStates.Available;
            }

            lock (LibrarySeatData.locker)
            {
                _librarySeatRepository.DbContext.SaveChanges();
            }
        }

        public (bool state, string message, Entities.LibrarySeat seat, Entities.OrderDetail order) RepeatOrder(Guid orderId, Guid userId)
        {
            var user = _userRepository.getById(userId);
            if (user == null)
                return (false, "用户不存在", null, null);

            var order = _orderRepository.getById(orderId);
            if (order == null)
                return (false, "订单不存在", null, null);

            if(order.EndTime> DateTime.Now.AddMinutes(20))
            {
                return (false, "必须在结束的前20分钟进行预定", null, null);
            }

            var seat = _librarySeatRepository.getById(order.LibrarySeatId);
            if (seat == null)
                return (false, "该座位不存在", null, null);

            if (!order.HasCheckIn)
                return (false, "你还未打卡不可以续订", null, null);

            if (order.HasEnd)
                return (false, "该订单以结束，请重新预定", null, null);

            if (order.SysUserId != userId)
                return (false, "订单与用户不匹配", null, null);


            order.EndTime.AddHours(2);
            _orderRepository.update(order);
            return (true, "续订成功", seat, order);
        }

        public IPagedList<Entities.LibrarySeat> searchSeat(LibrarySeatSearchArgs args, int page, int size)
        {
            var query = _librarySeatRepository.Table;
            if (args != null)
            {
                if (!String.IsNullOrEmpty(args.q))
                {
                    int seatnum;
                    bool isInt = int.TryParse(args.q, out seatnum);
                    if (isInt)
                    {
                        query = query.Where(o => o.SeatNumber == seatnum);
                    }
                }
            }
            query = from e in query orderby e.SeatNumber select e;
            return new PagedList<Entities.LibrarySeat>(query, page, size);
        }

        public Entities.LibrarySeat GetById(Guid id)
        {
            return _librarySeatRepository.getById(id);
        }

        public void InsertLibrarySeat(Entities.LibrarySeat librarySeat)
        {
            _librarySeatRepository.insert(librarySeat);
        }

        public void UpdateLibrarySeat(Entities.LibrarySeat librarySeat)
        {
            _librarySeatRepository.update(librarySeat);
        }

        public IList<Entities.LibrarySeat> GetLibrarySeats(int floor)
        {
            var query = _librarySeatRepository.Entities.Where(m => m.Floor == floor).ToList();
            return query;
        }

        public Entities.LibrarySeat GetBySeatNum(int seatNumber)
        {
            var query = _librarySeatRepository.Table;
            var list = query.ToList();
            query = query.Where(m => m.SeatNumber == seatNumber);
            if (query.Count() > 0)
            {
                var seat = query.First();
                return _librarySeatRepository.getById(seat.Id);
            }
                 
            return null;
        }
    }
}
