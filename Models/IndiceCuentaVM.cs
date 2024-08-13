namespace APP_Presupuesto.Models
{
    public class IndiceCuentaVM
    {
        public string TipoCuenta {  get; set; } 
        public IEnumerable<Cuentas> Cuentas { get; set; }
        public decimal Balance => Cuentas.Sum(x => x.Balance);
    }
}
