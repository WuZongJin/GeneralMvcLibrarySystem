using General.Core;
using General.Core.Data;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services
{
    public interface IOrderDetailService
    {
        OrderDetail GetBySeatNumberWithOutEnd(int seatnumber);
        IPagedList<Entities.OrderDetail> searchOrder(OrderDetailSearchArgs args, int page, int size);
        Entities.OrderDetail getById(Guid id);
        Entities.OrderDetail userCurrentOrder(Guid userId);
        IPagedList<Entities.OrderDetail> userHistoryOrder(Guid userId,int page,int size);
        void UpdateOrderdetail(Entities.OrderDetail orderDetail);
        void InsertOrdertail(Entities.OrderDetail orderDetail);

        void hasEnd(Guid id, bool hasEnd);
        void hasCheckIn(Guid id, bool hasCheckIn);
    }
}
