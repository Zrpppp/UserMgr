using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgr.Domain.ValueObjs;

namespace UserMgr.Domain.Entities
{
    public record UserLoginHistory: IAggregateRoot
    {
        public long Id { get; init; }
        public Guid? UserId {  get; init; }
        public PhoneNumber PhoneNumber { get; init; }
        public DateTime CreateDateTime {  get; init; }
        public string Message { get; init; }
        private UserLoginHistory() { }

        public UserLoginHistory(Guid? userid, PhoneNumber phoneNumber, string message)
        {
            this.UserId = userid;
            this.PhoneNumber = phoneNumber;
            this.Message = message;
            this.CreateDateTime = DateTime.Now;
        }
    }
}
