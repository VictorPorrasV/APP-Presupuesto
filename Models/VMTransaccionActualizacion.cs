namespace APP_Presupuesto.Models
{
    public class VMTransaccionActualizacion:VMTransaccionCreacion
    {
        public int cuentaAnteriorId {  get; set; }  
        public decimal MontoAnterior { get; set; }
        public string UrlRetorno { get; set; }


    }
}
