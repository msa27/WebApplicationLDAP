using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using System.Configuration;

namespace WebApplicationLDAP
{
    public class LDAPManager
    {
        #region Properties
        public DirectoryEntry entry { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Este metodo permite realizar una autenticacion de un usuario, 
        /// que pertenece a un grupo especifico.
        /// Para la autenticacion el usuario debe de ser miembro del grupo
        /// </summary>
        /// <param name="uid">Identificador del usuario</param>
        /// <param name="password">
        /// Contraseña de usuario, en el ldap debe de estar guardada con cifrado SHA1
        /// </param>
        /// <param name="group">Grupo del cual es miembro el usuario</param>
        /// <returns></returns>
        public bool Autenticar(String uid, String password, String group)
        {
            // Realizamos la conexión con el ldap
            entry = ConectarDirectoryEntry();

            if (entry == null)
            {
                // No se pudo realizar la conexion;
                return false;
            }

            if (Busqueda("(&(memberUid=" + uid + ")(cn=" + group + "))"))
            {
                if (Busqueda("(&(uid=" + uid + ")(userPassword=" + password + "))"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Este metodo permite obtener los valores de conexion del ldap
        /// requeridos para realizar operaciones de busqueda en el directorio
        /// </summary>
        /// <returns>El directoro de conexion</returns>
        public DirectoryEntry ConectarDirectoryEntry()
        {
            string ldap = ConfigurationManager.AppSettings["ldap"];
            string pwd = ConfigurationManager.AppSettings["pwd"];
            string usr = ConfigurationManager.AppSettings["usr"];
            try
            {
                DirectoryEntry entry = new DirectoryEntry(ldap, usr, pwd, AuthenticationTypes.FastBind);
                object nativeObject = entry.NativeObject;
                return entry;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Hace una busqueda en el LDAP
        /// </summary>
        /// <param name="Filtro">Busqueda</param>
        /// <returns>Si se encontro lo que se buscaba</returns>
        public bool Busqueda(String Filtro)
        {
            if (entry == null)
            {
                return false;
            }

            bool authenticated = false;
            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);
                search.PropertiesToLoad.Add("uid");
                search.Filter = Filtro;
                SearchResultCollection SRC = search.FindAll();

                if (SRC.Count > 1)
                {
                    // escribir en el log que hay mas de un resultado
                    return false;
                }

                if (SRC.Count == 0)
                {
                    // No se encontro el usuario con los datos especificados
                    return false;
                }

                if (SRC[0] != null)
                {
                    authenticated = true;
                }

            }
            catch (DirectoryServicesCOMException cex)
            {
                // Escribir en el log que ocurrio un error;
                return false;
            }
            catch (Exception ex)
            {
                // Escribir en el log que ocurrio un error;
                return false;
            }
            return authenticated;
        }

        /// <summary>
        /// Este metodo permite autenticar a un usuario en el sistema.
        /// No comprueba si pertenece a un grupo especifico.
        /// </summary>
        /// <param name="Usuario">Es el usuario que se va a autenticar</param>
        /// <param name="Contraseña">
        /// Contraseña del usuario a identificar, en el LDAP debe de estar
        /// encriptada con SHA1 para su comprobación
        /// </param>
        /// <returns>Si son validos los credenciales</returns>
        public bool IsAuthenticated(String Usuario, String Contraseña)
        {
            if (entry == null)
            {
                return false;
            }

            bool authenticated = false;
            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);
                search.PropertiesToLoad.Add("uid");
                //search.Filter = "(&(uid=moviedo)(userPassword={SHA}fEqNCco3Yq9h5ZUglD3CZJT4lBs=))";
                search.Filter = "(&(uid=" + Usuario + ")(userPassword=" + Contraseña + "))";
                SearchResultCollection SRC = search.FindAll();

                if (SRC.Count > 1)
                {
                    // escribir en el log que hay mas de un resultado
                    return false;
                }

                if (SRC.Count == 0)
                {
                    // No se encontro el usuario con los datos especificados
                    return false;
                }

                if (SRC[0] != null)
                {
                    authenticated = true;
                }

            }
            catch (DirectoryServicesCOMException cex)
            {
                // Escribir en el log que ocurrio un error;
                return false;
            }
            catch (Exception ex)
            {
                // Escribir en el log que ocurrio un error;
                return false;
            }
            return authenticated;
        }
        #endregion
    }
}