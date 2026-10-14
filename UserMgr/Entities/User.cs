using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgr.Domain.ValueObjs;
using Zack.Commons;

namespace UserMgr.Domain.Entities
{
    public record User: IAggregateRoot
    {
        public Guid Id { get; init; }
        public PhoneNumber PhoneNumber { get; private set; }
        private string? PasswordHash;
        public UserAccessFail UserAccessFail { get; init; }

        private User() { }

        public User(PhoneNumber phoneNumber)
        {
            this.PhoneNumber = phoneNumber;
            this.Id = Guid.NewGuid();
            this.UserAccessFail = new UserAccessFail(this);
        }

        public bool HasPassword()
        {
            return !string.IsNullOrEmpty(PasswordHash);
        }
        public void ChangePassword(string password)
        {
            if(password.Length <= 3)
            {
                throw new ArgumentOutOfRangeException("密码长度不能小于3");
            }
            this.PasswordHash = HashHelper.ComputeMd5Hash(password);
        }

        public bool ValidatePassword(string password)
        {
            return this.PasswordHash == HashHelper.ComputeMd5Hash(password);
        }

        public void changePhoneNumber(PhoneNumber phoneNumber)
        {
            this.PhoneNumber = phoneNumber;
        }
    }
}
