using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("libraryseat")]
    public class LibrarySeat
    {
        public LibrarySeat()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }                    //数据库主键
        [Required]
        public int SeatNumber { get; set; }             //座位编号
        [Required]
        public int Floor { get; set; }                  //座位所在楼层        
        [Required]
        public SeatStates SeatState { get; set; }       //座位状态

        public enum SeatStates
        {
            Available = 0,
            Booked = 1,
            InAvailable = 2,
        }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
