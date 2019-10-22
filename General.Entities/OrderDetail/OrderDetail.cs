using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public Guid Id { get; set; }                        //数据库主键
        [Required]
        public Guid SysUserId { get; set; }                 //订单对应的用户
        [Required]
        public Guid LibrarySeatId { get; set; }             //订单对应的座位
        [Required]
        public DateTime CreateTime { get; set; }            //订单创建的时间
        [Required]
        public DateTime EndTime { get; set; }               //订单结束的时间
        [Required]
        public string VerificationCode { get; set; }        //验证码
        [Required]
        public bool HasCheckIn { get; set; }                 //是否已经打卡
        [Required]
        public bool HasEnd { get; set; }                    //是否已经结束


    }
}
