using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Classes
{
    class photo
    {
        public int idUser;
        public string photoPhat;

        public photo(int idUser, string photoPhat)
        {
            this.idUser = idUser;
            this.photoPhat = photoPhat;
        }
    }
}
