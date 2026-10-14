using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgr.Domain;
using UserMgr.Domain.Entities;
using UserMgr.Domain.ValueObjs;
using Zack.Infrastructure.EFCore;

namespace UserMgr.Infrastracture
{
   
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext dbContext;
        private readonly IDistributedCache distributedCache;
        private readonly IMediator mediator;

        public UserRepository(UserDbContext dbContext, IDistributedCache distributedCache, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.distributedCache = distributedCache;
            this.mediator = mediator;
        }
        public async Task AddNewLoginHistory(PhoneNumber phoneNumber, string message)
        {
            User? user = await FindOneAsync(phoneNumber);
            Guid? userId = null;
            if (user != null)
            {
                userId = user.Id;
            }
            dbContext.UserLoginHistory.Add(new UserLoginHistory(userId, phoneNumber, message));
        }

        public async Task<User?> FindOneAsync(PhoneNumber phoneNumber)
        {
            User? user = await dbContext.Users.SingleOrDefaultAsync(ExpressionHelper.MakeEqual((User u) =>
            u.PhoneNumber, phoneNumber));
            return user;

        }

        public async Task<User?> FindOneAsync(Guid userId)
        {;
            User? user = await dbContext.Users.SingleOrDefaultAsync(U => U.Id == userId));
            return user;
        }

        public async Task<string?> FindPhoneNumberCodeAsync(PhoneNumber phoneNumber)
        {
            string key = $"PhoneNumberCode_{phoneNumber.RegionNumber}_{phoneNumber.Number}";
            string? code = await distributedCache.GetStringAsync(key);
            distributedCache.Remove(key);//删除
            return code;
        }

        public Task PublishEventAsync(UserAccessResultEvent _event)
        {
            return mediator.Publish(_event);
        }

        public Task SavePhoneNumberCodeAsync(PhoneNumber phoneNumber, string code)
        {
            //缓存
            string key = $"PhoneNumberCode_{phoneNumber.RegionNumber}_{phoneNumber.Number}";
            return distributedCache.SetStringAsync(key, code,new DistributedCacheEntryOptions
            { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)});
        }
    }
}
