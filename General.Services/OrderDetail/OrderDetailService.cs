using General.Core;
using General.Core.Data;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services
{
    public class OrderDetailService:IOrderDetailService
    {
        private IRepository<OrderDetail> _orderdetailRepository;
        private IRepository<Entities.SysUser> _sysUserRepository;
        private IRepository<Entities.LibrarySeat> _librarySeatRepository;

        public OrderDetailService(IRepository<OrderDetail> orderdetailRepository,IRepository<Entities.SysUser> sysUserRepository,IRepository<Entities.LibrarySeat> librarySeatRepository)
        {
            _sysUserRepository = sysUserRepository;
            _orderdetailRepository = orderdetailRepository;
            _librarySeatRepository = librarySeatRepository;
        }

        public OrderDetail getById(Guid id)
        {
            return _orderdetailRepository.getById(id);
        }

        public OrderDetail GetBySeatNumberWithOutEnd(int seatnumber)
        {
            
            var query = _librarySeatRepository.Table.Where(m => m.SeatNumber == seatnumber);
            if (query.Count() <= 0)
                return null;
            var seatid = query.First().Id;
            var order = _orderdetailRepository.Table.Where(m => m.LibrarySeatId == seatid&&m.HasEnd == true);
            if (order.Count() > 0)
            {
                return order.First();
            }

            return null;
        }

        public void hasCheckIn(Guid id, bool hasCheckIn)
        {
            var order = _orderdetailRepository.getById(id);
            if (order != null)
            {
                order.HasCheckIn = hasCheckIn;
                _orderdetailRepository.update(order);
            }
        }

        public void hasEnd(Guid id, bool hasEnd)
        {
            var order = _orderdetailRepository.getById(id);
            if (order != null)
            {
                order.HasEnd = hasEnd;
                _orderdetailRepository.update(order);
            }
        }

        public void InsertOrdertail(OrderDetail orderDetail)
        {
            _orderdetailRepository.insert(orderDetail);
        }

        public IPagedList<OrderDetail> searchOrder(OrderDetailSearchArgs args, int page, int size)
        {
            var query = _orderdetailRepository.Table;
            if(args!=null)
            {
                if (!String.IsNullOrEmpty(args.q))
                {
                    var userList = _sysUserRepository.Entities.Where(o => o.Account==(args.q));
                    Entities.SysUser user = null;
                    if (userList.Count() > 0)
                    {
                        user = userList.First();
                    }
                    int seatNumber;
                    Entities.LibrarySeat seat = null;
                    bool isInt = int.TryParse(args.q, out seatNumber);
                    if (isInt)
                    {
                        var seatList = _librarySeatRepository.Entities.Where(o => o.SeatNumber == seatNumber);
                        if (seatList.Count() > 0)
                            seat = seatList.First();
                    }
                    ;
                    if (user != null&& seat != null)
                    {
                        query = query.Where(o => o.SysUserId == user.Id|| o.LibrarySeatId == seat.Id);
                    }
                    else if(user != null)
                    {
                        query = query.Where(o => o.SysUserId == user.Id);
                    }
                    else if(seat != null)
                    {
                        query = query.Where(o => o.LibrarySeatId == seat.Id);
                    }
                }
                if (args.hascheckin.HasValue)
                {
                    query = query.Where(o => o.HasCheckIn == args.hascheckin);
                }
                if (args.hasend.HasValue)
                {
                    query = query.Where(o => o.HasEnd == args.hasend);
                }
                    
            }
            query = from e in query orderby e.CreateTime descending select e;
            return new PagedList<Entities.OrderDetail>(query, page, size);
        }

        public void UpdateOrderdetail(OrderDetail orderDetail)
        {
            _orderdetailRepository.update(orderDetail);
        }

        public OrderDetail userCurrentOrder(Guid userId)
        {
            var user = _sysUserRepository.getById(userId);
            if (user == null)
                return null;
            var query = _orderdetailRepository.Entities
                .Where(m => m.SysUserId == user.Id)
                .Where(m => !m.HasEnd);
            if(query.Count()>0)
                return query.First();

            return null;
        }

        public IPagedList<OrderDetail> userHistoryOrder(Guid userId,int page,int size)
        {
            _librarySeatRepository.DbContext.SaveChanges();
            var user = _sysUserRepository.getById(userId);
            if (user == null)
                return null;
            var query = _orderdetailRepository.Entities
                .Where(m => m.SysUserId == user.Id)
                .Where(m => m.HasEnd);

            query = query.OrderBy(m=>m.CreateTime);
            return new PagedList<Entities.OrderDetail>(query, page, size);
        }
    }
}
