using AutoMapper;
using ManejadorDePresupuestos.Models;

namespace ManejadorDePresupuestos.Services
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
            CreateMap<TransaccionActualizacionViewModel,Transaccion>().ReverseMap();
        }
    }
}
