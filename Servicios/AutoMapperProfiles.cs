using APP_Presupuesto.Models;
using AutoMapper;

namespace APP_Presupuesto.Servicios
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {

            CreateMap<Cuentas, CuentasVM>();
            CreateMap<VMTransaccionActualizacion, Transacciones>();
            CreateMap<Transacciones, VMTransaccionActualizacion>();
        }
    }
}
