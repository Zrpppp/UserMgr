using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgr.Domain.Entities;
using UserMgr.Domain.ValueObjs;

namespace UserMgr.Domain
{
    public class UserDomainService
    {
        private IUserRepository userRepository;
        private ISmsCodeSender smsCodeSender;
        public UserDomainService(IUserRepository userRepository, ISmsCodeSender smsCodeSender)
        {
            this.userRepository = userRepository;
            this.smsCodeSender = smsCodeSender;
        }

        public void ResetAccessFail(User user)
        {
            user.UserAccessFail.Reset();
        }

        public bool IsLockOut(User user)
        {
            return user.UserAccessFail.IsLockOut();
        }

        public void AccessFail(User user)
        {
            user.UserAccessFail.Fail();
        }

        public async Task<UserAccessResult> CheckPassword(PhoneNumber phoneNumber, string password)
        {
            UserAccessResult result;
            var user = await userRepository.FindOneAsync(phoneNumber);
            if (user == null)
            {
                return UserAccessResult.PhoneNumberNotFound;
            }
            else if (IsLockOut(user))
            {
                result = UserAccessResult.Lockout;
            }
            else if (!user.HasPassword())
            {
                result = UserAccessResult.NoPassword;
            }
            else if (user.ValidatePassword(password))
            {
                result = UserAccessResult.OK;
            }
            else
            {
                result = UserAccessResult.PasswordError;
            }
            if (user != null) {
                if (result == UserAccessResult.OK)
                {
                    ResetAccessFail(user);
                }
                else
                {
                    AccessFail(user);
                }
            }
            await userRepository.PublishEventAsync(new UserAccessResultEvent(phoneNumber, result));
            return result;
        }

        public async Task<CheckCodeResult> CheckPhoneNumberCodeAsync(PhoneNumber phoneNumber, string code)
        {
            User? user = await userRepository.FindOneAsync(phoneNumber);
            if (user == null) return CheckCodeResult.PhoneNumberNotFound;
            if (IsLockOut(user)) return CheckCodeResult.Lockout;
            string? phoneNumberCode = await userRepository.FindPhoneNumberCodeAsync(phoneNumber);
            if (phoneNumberCode == null) {
                AccessFail(user);
               return CheckCodeResult.CodeError;
            } 
            if (phoneNumberCode == code)
            {
                return CheckCodeResult.OK;
            }
            else
            {
                AccessFail(user);
                return CheckCodeResult.CodeError;
            }
        }
    }
}
