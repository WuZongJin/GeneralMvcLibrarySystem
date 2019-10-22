using General.Core.Data;
using General.Services.LibrarySeat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static General.Entities.LibrarySeat;

namespace General.Services
{
    public interface IUpdateLibrarySeatStateService
    {
        void Run();
        void UpdateSeatState();
    }

    public class UpdateLibrarySeatService : IUpdateLibrarySeatStateService
    {
        private IRepository<Entities.LibrarySeat> _librarySeatRepository;
        private IRepository<Entities.OrderDetail> _orderRepository;
        private IRepository<Entities.SysUser> _sysUserRepository;
        public UpdateLibrarySeatService(IRepository<Entities.LibrarySeat> libraryRepository, IRepository<Entities.OrderDetail> orderRepository,IRepository<Entities.SysUser> userRepository)
        {
            _librarySeatRepository = libraryRepository;
            _orderRepository = orderRepository;
            _sysUserRepository = userRepository;
            
        }

        public void Run()
        {

            UpdateSeatState();
            
            
        }

        public async void UpdateSeatState()
        {
            var orderCheckInTimeOut = _orderRepository.Table
                .Where(o => o.HasEnd == false)
                .Where(o => o.HasCheckIn == false && DateTime.Now >= o.CreateTime.AddMinutes(LibrarySeatData.CheckInTime));
            foreach (var order in orderCheckInTimeOut)
            {
                order.HasEnd = true;
               
                var seat = _librarySeatRepository.getById(order.LibrarySeatId);
                seat.SeatState = SeatStates.Available;
                

                var user = _sysUserRepository.getById(order.SysUserId);
                user.ViolationNum++;
                if (user.ViolationNum >= LibrarySeatData.ViolationMaxNum)
                {
                    user.ScheduledLock = true;
                    user.AllowScheduleTime = DateTime.Now.AddDays(3);
                }
            }
            lock (LibrarySeatData.locker)
            {
                var ta = _librarySeatRepository.DbContext.SaveChangesAsync();
                ta.Wait();
            }
           


            var orderEndTimeOut = _orderRepository.Table
                .Where(o => o.HasEnd == false)
                .Where(o => DateTime.Now >= o.CreateTime.AddHours(LibrarySeatData.OrderEndTime));

            var shouldChangeStateorder = orderEndTimeOut.Union(orderCheckInTimeOut);

            foreach (var order in shouldChangeStateorder)
            {
                order.HasEnd = true;
                var seat = _librarySeatRepository.getById(order.LibrarySeatId);
                seat.SeatState = SeatStates.Available;
            }


            lock (LibrarySeatData.locker)
            {
                var ta1 =_librarySeatRepository.DbContext.SaveChangesAsync();
                ta1.Wait();
            }




        }


        public void Update()
        {
             UpdateSeatState();
        }

    }
}
