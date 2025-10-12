using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MiLogica.Utils;

namespace MiLogica.ModeloDatos
{

    public class Usuario
    {
        // Ejemplo de mejora
        private const int MAX_INTENTOS_FALLIDOS = 3;
        private const int MINUTOS_VENTANA_INTENTOS = 5;
        private const int DIAS_PARA_INACTIVIDAD = 182;

        public string _nombre;
        public string _apellidos;
        public string _email;
        public string _passwordHash;
        public int Id { get; set; }
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El nombre no puede estar vacío.");
                }
                _nombre = value;
            }
        }
        public string Apellidos
        {
            get => _apellidos;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Los apellidos no pueden estar vacíos.");
                }
                _apellidos = value;
            }
        }
        public bool Suscripcion { get; set; }

        public string Email
        {
            get => _email;
            set
            {
                if (!Utils.Email.ValidarEmail(value))
                {
                    throw new ArgumentException("El formato del email no es válido.");
                }
                _email = value;
            }
        }


        public DateTime LastLogin { get;  set; }


        //Atributos para la lógica de bloqueo
        public EstadoUsuario Estado { get;  set; }

        public List<DateTime> intentosFallidosTimestamps;

        public Usuario() 
        {
            this.Email = "example@gmail.com";
            this.Nombre = "nameToChange";
            this.Apellidos = "surnamesToChange";
            this._passwordHash = string.Empty;
            this.intentosFallidosTimestamps = new List<DateTime>();
        }

        public Usuario(int id, string nombre, string password, string apellidos, string email, bool suscripcion)
        {

            if (!Utils.Email.ValidarEmail(email))
            {
                throw new ArgumentException("El formato del email no es valido");
            }
            if (!Utils.Password.ValidarPassword(password))
            {
                throw new ArgumentException("La contraseña no cumple los requisitos de seguridad.");
            }
            this.Id = id;
            this.Nombre = nombre;
            this.Apellidos = apellidos;
            this.Email = email;
            this.Suscripcion = suscripcion;

            this._passwordHash = Encriptar.EncriptarPasswordSHA256(password);

            this.Estado = EstadoUsuario.Activo;
            this.intentosFallidosTimestamps = new List<DateTime>(); // ¡Esta es la línea clave!
            this.LastLogin = DateTime.Now;

        }

        public bool PermitirLogin(string passwordDado)
        {

            VerificarInactividad();
            if (this.Estado == EstadoUsuario.Bloqueado) return false;

            string passwordEncriptada = Encriptar.EncriptarPasswordSHA256(passwordDado);
            bool esPasswordCorrecta = this._passwordHash.Equals(passwordEncriptada);
            bool estabaInactivo = (this.Estado == EstadoUsuario.Inactivo);

            if (esPasswordCorrecta)
            {
                this.intentosFallidosTimestamps.Clear();
                Estado = EstadoUsuario.Activo;
                this.LastLogin = DateTime.Now;
                return true;
            }
            else
            {
                if (estabaInactivo)
                {
                    this.Estado = EstadoUsuario.Bloqueado; // Bloqueo inmediato
                    this.intentosFallidosTimestamps.Clear(); // Limpiamos por si acaso
                    return false;
                }
                this.intentosFallidosTimestamps.Add(DateTime.Now);

                var now = DateTime.Now;
                this.intentosFallidosTimestamps = this.intentosFallidosTimestamps
                    .Where(t => (now - t).TotalMinutes <= MINUTOS_VENTANA_INTENTOS)
                    .ToList();

                if (this.intentosFallidosTimestamps.Count >= MAX_INTENTOS_FALLIDOS) 
                {
                    this.Estado = EstadoUsuario.Bloqueado;
                }
            }
            return false;

        }

        public bool ComprobarPassWord(string passwordAComprobar)
        {
            return ComprobarHash(passwordAComprobar);
        }


        
        public bool DesbloquearUsuario (string email, string passwordDado )
        {
            if (this.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && (this.Estado == EstadoUsuario.Bloqueado || this.Estado == EstadoUsuario.Inactivo) && ComprobarHash(passwordDado)) 
            { 
                RestablecerCuenta();
                return true;
            }
            return false;
        }


        public void VerificarInactividad()
        {
            if (this.Estado == EstadoUsuario.Activo)
            {
                TimeSpan tiempoSinAcceder = DateTime.Now - this.LastLogin;
                if(tiempoSinAcceder > TimeSpan.FromDays(DIAS_PARA_INACTIVIDAD)) // Aproximadamente 6 meses
                {
                    this.Estado = EstadoUsuario.Inactivo;
                }
            }
        }

        public bool CambiarPassword(string passwordActual, string nuevoPassword)
        {
            if (this.Estado == EstadoUsuario.Bloqueado)
            {
                return false;
            }
            if (!ComprobarHash(passwordActual) || !Utils.Password.ValidarPassword(nuevoPassword))
            {
                return false;
            }
            this._passwordHash = Encriptar.EncriptarPasswordSHA256(nuevoPassword);
            this.RestablecerCuenta();
            return true;
        }

        private void RestablecerCuenta()
        {
            this.Estado = EstadoUsuario.Activo;
            this.intentosFallidosTimestamps.Clear();
            this.LastLogin = DateTime.Now;
        }

        public void ActualizarPerfil(string nombre, string apellidos, bool suscripcion)
        {
            this.Nombre = nombre;
            this.Apellidos = apellidos;
            this.Suscripcion = suscripcion;
        }

        private bool ComprobarHash(string passwordAComprobar)
        {
            string passwordEncriptada = Encriptar.EncriptarPasswordSHA256(passwordAComprobar);
            return this._passwordHash == passwordEncriptada;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Nombre: {Nombre}, Apellidos: {Apellidos}, Email: {Email}, Suscripción: {Suscripcion}, Estado: {Estado}, Último Login: {LastLogin}";
        }

    }
}
