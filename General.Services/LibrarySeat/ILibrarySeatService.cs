using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.LibrarySeat
{
    public interface ILibrarySeatService
    {
        Entities.LibrarySeat GetById(Guid id);
        Entities.LibrarySeat GetBySeatNum(int seatNumber);
        void InsertLibrarySeat(Entities.LibrarySeat librarySeat);
        void UpdateLibrarySeat(Entities.LibrarySeat librarySeat);

        IList<Entities.LibrarySeat> GetALLLibrarySeat();                     //获取所有的座位信息
        IList<Entities.LibrarySeat> GetLibrarySeats(int floor);             //获取当前楼层座位
        IList<Entities.LibrarySeat> GetLibrarySetsAvailable(int floor);      //获取当前楼层信息
        IList<Entities.LibrarySeat> GetLibrarySetsInAvailable(int floor);    //获取当前可预订座位信息
        IList<Entities.LibrarySeat> GetLibrarySetsBooked(int floor);         //获取以预定座位信息
        (int avilable, int booked, int inavilable) GetLibrarySeatCount(int floor);      //获取当前楼层可用，被预定，被坐的座位数量
        (bool state, string message, Entities.LibrarySeat seat, OrderDetail order) Schedule(int seatNumber, Guid userId);        //预定座位
        (bool state, string message, Entities.LibrarySeat seat, OrderDetail order) RepeatOrder(Guid orderId, Guid userId);                //续订座位
        IPagedList<Entities.LibrarySeat> searchSeat(LibrarySeatSearchArgs args, int page, int size);

        void UpdateSeatState();         //更新座位信息

    }
}
