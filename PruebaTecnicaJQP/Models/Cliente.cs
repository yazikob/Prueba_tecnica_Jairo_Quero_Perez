using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PruebaTecnicaJQP.Models
{
    [Serializable]
    public class Cliente
    {
        public int idCliente { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }
        public string nombres { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string sexo { get; set; }
    }
}