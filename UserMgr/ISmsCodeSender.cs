using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgr.Domain.ValueObjs;

namespace UserMgr.Domain
{
    public interface ISmsCodeSender
    {
        public Task SendAsync(PhoneNumber phoneNumber, string code);
    }
}
