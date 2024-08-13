namespace APP_Presupuesto.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }


        public IEnumerable<TransaccionesPorFecha> transaccionesAgrupadas { get; set; }  

        public decimal BalanceDepósitos => transaccionesAgrupadas.Sum(x => x.BalanceDepositos); 

        public decimal BalanceRetiros => transaccionesAgrupadas.Sum(x => x.BalanceRetiros);

        public decimal Total => BalanceDepósitos -BalanceRetiros;

        public class TransaccionesPorFecha
        {

            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transacciones>transacciones { get; set; }
            public decimal BalanceDepositos => transacciones.Where
                (x => x.TipoOperacionId == TipoOperaciones.Ingreso)
                .Sum(x => x.Monto);
            public decimal BalanceRetiros => transacciones.Where
              (x => x.TipoOperacionId == TipoOperaciones.Gasto)
              .Sum(x => x.Monto);
        }

    }
}
