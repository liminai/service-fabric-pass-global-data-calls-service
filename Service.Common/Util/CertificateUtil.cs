using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Common.Util
{
    public static class CertificateUtil
    {
        public static X509Certificate2 GetCertificate(string storeName, string thumbprint)
        {
            if (!string.IsNullOrEmpty(thumbprint))
            {
                if (string.IsNullOrEmpty(storeName))
                {
                    storeName = StoreName.My.ToString();
                }
                X509Store store = new X509Store(storeName, StoreLocation.CurrentUser);
                X509Certificate2 cert = GetCertificateFromStore(store, thumbprint);
                if (cert == null)
                {
                    store = new X509Store(storeName, StoreLocation.LocalMachine);
                    cert = GetCertificateFromStore(store, thumbprint);
                }
                return cert;
            }
            return null;
        }

        private static X509Certificate2 GetCertificateFromStore(X509Store store, string thumbprint)
        {
            try
            {
                store.Open(OpenFlags.ReadOnly);
                thumbprint = Regex.Replace(thumbprint, @"\s+", "").ToUpper();
                X509Certificate2Collection coll = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (coll.Count > 0)
                {
                    return coll[0];
                }
            }
            finally
            {
                store.Close();
            }
            return null;
        }
    }
}
