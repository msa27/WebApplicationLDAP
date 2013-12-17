using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace WebApplicationLDAP
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LDAPManager LDAPM = new LDAPManager();
            bool a = LDAPM.Autenticar("misalazar", "{SHA}fEqNCco3Yq9h5ZUglD3CZJT4lBs=", "AdminConsejo");

            //string ldap = "LDAP://192.168.1.250/ou=people,dc=ic-itcr,dc=ac,dc=cr";
            //string pwd = "123456";          
            //string ldap = "LDAP://172.19.127.19/ou=people,dc=ic-itcr,dc=ac,dc=cr";
            //string pwd = "Solaris2013";
            string ldap = "LDAP://192.168.0.112/dc=ic-itcr,dc=ac,dc=cr";
            string pwd = "123456";
            string usr = "cn=admin,dc=ic-itcr,dc=ac,dc=cr";
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

                DirectorySearcher search = new DirectorySearcher(entry);

                search.PropertiesToLoad.Add("userPassword");
                search.PropertiesToLoad.Add("uid");
                //search.PropertiesToLoad.Add("memberOf");                
                //search.Filter = ("(|(objectClass=inetOrgPerson)(objectCategory=user))");
                //search.Filter = "(&(objectCategory=user)(memberOf=cn=ests,ou=people,dc=ic-itcr,dc=ac,dc=cr))";
                //search.SizeLimit = 1000; 
                //search.Filter = "(memberOf=cn=ests,ou=group,dc=ic-itcr,dc=ac,dc=cr)";
                search.Filter = "(&(memberUid=misalazar2)(cn=users))";
                foreach (SearchResult result in search.FindAll())
                {
                    if (result != null)
                    {
                        DirectoryEntry de = result.GetDirectoryEntry();
                        object x = de.Properties;
                        string uid = de.Properties["userPassword"].Value.ToString();
                        uid = uid.Trim();
                        uid = uid.ToLower();
                    }
                }

                DirectorySearcher searcher = new DirectorySearcher(entry);
                searcher.Filter = "(uid=misalazar)";
                searcher.PropertiesToLoad.Add("uid");

                SearchResult result2 = searcher.FindOne();
                if (result2 != null)
                    Console.WriteLine("Found");
                else
                    Console.WriteLine("Not found");
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