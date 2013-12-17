using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;

namespace WebApplicationLDAP
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ldap = "LDAP://172.19.127.19/ou=people,dc=ic-itcr,dc=ac,dc=cr";
            string usr = "cn=admin,dc=ic-itcr,dc=ac,dc=cr";
            string pwd = "Solaris2013";
            IsAuthenticated(ldap, usr, pwd);
        }
       
 
        public static bool IsAuthenticated(string ldap, string usr, string pwd)
        {
            bool authenticated = false;
 
            try
            {
                DirectoryEntry entry = new DirectoryEntry(ldap, usr, pwd, AuthenticationTypes.FastBind);
                object nativeObject = entry.NativeObject;
                authenticated = true;
            }
            catch (DirectoryServicesCOMException cex)
            {
                Console.WriteLine(cex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return authenticated;
        }
    }
}