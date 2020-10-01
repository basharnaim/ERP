using System;
using System.Security.Principal;

namespace Library.Crosscutting.Securities
{
    public class LoginIdentity : IIdentity
    {
        public LoginIdentity(string basicTicket)
        {
            string[] ticketData = basicTicket.Split(new string[] { "__#" }, StringSplitOptions.None);
            Name = ticketData[0];
            CompanyId = ticketData[1];
            CompanyName = ticketData[2];
            BranchId = ticketData[3];
            BranchName = ticketData[4];
            IpAddress = ticketData[5];
            SysAdmin = Convert.ToBoolean(ticketData[6]);
            IsAuthenticated = true;
        }

        public string Name { get; }
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string BranchId { get; private set; }
        public string BranchName { get; private set; }
        public string IpAddress { get; private set; }
        public bool SysAdmin { get; private set; }
        public bool IsAuthenticated { get; }
        public string AuthenticationType { get { return "UYAuthentication"; } }
        public string[] PermittedRoles { get; private set; }

        public static string CreateBasicTicket(
                                            string name,
                                            string companyId,
                                            string companyName,
                                            string branchId,
                                            string branchName,
                                            string ipAddress,
                                            bool sysAdmin,
                                            bool isAuthenticated
            )
        {
            return name + "__#"
                + companyId + "__#"
                + companyName + "__#"
                + branchId + "__#"
                + branchName + "__#"
                + ipAddress + "__#"
                + sysAdmin + "__#"
                + sysAdmin + "__#"
                + isAuthenticated + "__#"
                ;
        }

        public static string CreateRoleTicket(string[] roles)
        {
            string rolesString = "";
            for (int i = 0; i < roles.Length; i++)
            {
                rolesString += roles[i] + ",";
            }
            rolesString.TrimEnd(new char[] { ',' });

            return rolesString + "__#";
        }

        public void SetRoles(string roleTicket)
        {
            PermittedRoles = roleTicket == "" ? new string[0] : roleTicket.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
