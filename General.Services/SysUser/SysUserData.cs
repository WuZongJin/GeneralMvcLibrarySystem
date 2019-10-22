using General.Core.Librs;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services
{
    public static class SysUserData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using(var context = new GeneralDbContext(serviceProvider.GetRequiredService<DbContextOptions<GeneralDbContext>>()))
            {
                if (context.SysUsers.Any())
                {
                    return;
                }
                var salt = EncryptorHelper.CreateSaltKey();

                context.SysUsers.AddRange(
                    new Entities.SysUser
                    {
                        Id = Guid.NewGuid(),
                        Account = "41606217",
                        Name = "吴宗锦",
                        Salt = salt,
                        Password = EncryptorHelper.GetMD5("15160296867" + salt),
                        IsAdmin = true,
                        Email = "1395080043@qq.com",
                        MobilePhone = "18229065977",


                        Sex = "男",
                        Enabled = true,
                        CreationTime = DateTime.Now,
                        LoginFailedNum = 0,
                        AllowLoginTime = null,
                        LoginLock = false,
                        LastLoginTime = null,
                        LastIpAddress="",
                        LastActivityTime = DateTime.Now,
                        IsDeleted = false,
                        DeletedTime = null,
                        ModifiedTime = null,
                        Modifier = null,
                        Creator = null,
                        Avatar=new byte[0],
                    }
                        );
                context.SaveChanges();

            }
        }


    }
}
