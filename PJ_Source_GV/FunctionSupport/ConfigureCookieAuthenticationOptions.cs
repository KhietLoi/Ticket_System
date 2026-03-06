using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.FunctionSupport
{
    public class ConfigureCookieAuthenticationOptions: IPostConfigureOptions<CookieAuthenticationOptions>
    {
        private readonly ITicketStore _ticketStore;

        public ConfigureCookieAuthenticationOptions(ITicketStore ticketStore)
        {
            _ticketStore = ticketStore;
        }

        public void PostConfigure(string name,
                 CookieAuthenticationOptions options)
        {
            options.SessionStore = _ticketStore;
        }
    }
}
