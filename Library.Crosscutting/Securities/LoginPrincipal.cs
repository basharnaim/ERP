using System.Linq;
using System.Security.Principal;

namespace Library.Crosscutting.Securities
{
    public class LoginPrincipal : IPrincipal
    {
        public LoginPrincipal(LoginIdentity identity)
        {
            Identity = identity;
        }

        public IIdentity Identity { get; private set; }

        public bool IsInRole(string roleId)
        {
            return ((LoginIdentity)Identity).PermittedRoles.Any(x => x.Contains(roleId));
        }
    }
}
