using MiLogica.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
        public int? _edad;
        public double? _peso;
        public DateTime? BloqueadoHasta { get; set; }
        public int Id { get; set; }

        public double? Peso // Cambiado a double?
        {
            get => _peso;
            set
            {
                // Validar solo si se proporciona un valor
                if (value.HasValue && (value < 0 || value > 500))
                {
                    throw new ArgumentException("El peso debe estar entre 0 y 500 kg.");
                }
                _peso = value; // Asignar el valor (puede ser null)
            }
        }

        // --- PROPIEDAD EDAD (AHORA int?) ---
        public int? Edad // Cambiado a int?
        {
            get => _edad;
            set
            {
                // Validar solo si se proporciona un valor
                if (value.HasValue && (value < 0 || value > 120))
                {
                    throw new ArgumentException("La edad debe estar entre 0 y 120 años.");
                }
                _edad = value; // Asignar el valor (puede ser null)
            }
        }
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El nombre no puede estar vacío.");
                }
                if (!Utils.Valid.Nombre(value))
                {
                    throw new ArgumentException("El nombre no puede contener números u otros caracteres no válidos.");
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
                    throw new ArgumentException("El apellido no puede estar vacío.");
                }
                if (!Utils.Valid.Nombre(value))
                {
                    throw new ArgumentException("El apellido no puede contener números u otros caracteres no válidos.");
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
            _edad = null; 
            _peso = null; 
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
            _edad = null; 
            _peso = null; 

            this._passwordHash = Encriptar.EncriptarPasswordSHA256(password);

            this.Estado = EstadoUsuario.Activo;
            this.intentosFallidosTimestamps = new List<DateTime>(); // ¡Esta es la línea clave!
            this.LastLogin = DateTime.Now;

        }

        public bool PermitirLogin(string passwordDado)
        {
            Console.WriteLine($"[DEBUG-LOGIN] Intento para {this.Email}. Estado en DB: {this.Estado}");
            VerificarInactividad();
            if (this.Estado == EstadoUsuario.Bloqueado && this.BloqueadoHasta.HasValue && DateTime.Now < this.BloqueadoHasta) return false;

            string passwordEncriptada = Encriptar.EncriptarPasswordSHA256(passwordDado);
            bool esPasswordCorrecta = this._passwordHash.Equals(passwordEncriptada);
            bool estabaInactivo = (this.Estado == EstadoUsuario.Inactivo);

            if (esPasswordCorrecta)
            {
                Console.WriteLine($"[DEBUG-LOGIN] Contraseña CORRECTA. Estado antes del acceso: {this.Estado}");

                if (this.Estado == EstadoUsuario.Bloqueado)
                {
                    Console.WriteLine($"[DEBUG-LOGIN] ACCESO DENEGADO. Motivo: Estado no es Activo ({this.Estado})");
                    return false;
                }

                this.intentosFallidosTimestamps.Clear();
                Estado = EstadoUsuario.Activo;
                this.LastLogin = DateTime.Now;
                Console.WriteLine($"[DEBUG-LOGIN] ACCESO CONCEDIDO. Estado final: {this.Estado}");
                return true;
            }
            else
            {
                Console.WriteLine($"[DEBUG-LOGIN] Contraseña INCORRECTA. Estado final después del fallo: {this.Estado}");

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
                    this.BloqueadoHasta = DateTime.Now.AddMinutes(2);
                }
            }
            return false;

        }

        public bool ComprobarPassWord(string passwordAComprobar)
        {
            string passwordEncriptada = Encriptar.EncriptarPasswordSHA256(passwordAComprobar);
            return this._passwordHash == passwordEncriptada;
        }


        
        public bool DesbloquearUsuario (string email, string passwordDado )
        {
            if (this.Estado == EstadoUsuario.Bloqueado && DateTime.Now < this.BloqueadoHasta) return false;
            if (this.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && (this.Estado == EstadoUsuario.Bloqueado || this.Estado == EstadoUsuario.Inactivo) && ComprobarPassWord(passwordDado)) 
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
            if (!ComprobarPassWord(passwordActual) || !Utils.Password.ValidarPassword(nuevoPassword))
            {
                return false;
            }
            this._passwordHash = Encriptar.EncriptarPasswordSHA256(nuevoPassword);
            this.RestablecerCuenta();
            return true;
        }

        /// <summary>
        /// Permite a un administrador establecer una nueva contraseña para un usuario.
        /// Omite la comprobación de la contraseña actual, pero SÍ valida la seguridad de la nueva.
        /// También restablece la cuenta del usuario a Activo.
        /// </summary>
        /// <param name="nuevoPassword">La nueva contraseña segura.</param>
        /// <returns>True si la contraseña era segura y se estableció; false en caso contrario.</returns>
        public bool AdminEstablecerPassword(string nuevoPassword)
        {
            // Un admin SIEMPRE puede cambiar la pass, sin importar el estado (Bloqueado, etc.)
            // PERO la nueva password DEBE ser segura.
            if (!Utils.Password.ValidarPassword(nuevoPassword))
            {
                return false; // La nueva password no es segura
            }

            this._passwordHash = Encriptar.EncriptarPasswordSHA256(nuevoPassword);

            // Al resetear la password, es lógico también restablecer la cuenta a 'Activo'.
            this.RestablecerCuenta(); // Esto ya pone Estado=Activo, limpia intentos, etc.
            return true;
        }

        private void RestablecerCuenta()
        {
            this.Estado = EstadoUsuario.Activo;
            this.intentosFallidosTimestamps.Clear();
            this.LastLogin = DateTime.Now;
            this.BloqueadoHasta = null;
        }

        public void ActualizarPerfil(string nombre, string apellidos, int? edad, double? peso)
        {
            this.Nombre = nombre;
            this.Apellidos = apellidos;

            this.Edad = edad;
            this.Peso = peso;


        }




        public override string ToString()
        {
            string infoExtra = "";
            if (Edad.HasValue) infoExtra += $", Edad: {Edad.Value}";
            if (Peso.HasValue) infoExtra += $", Peso: {Peso.Value.ToString(CultureInfo.InvariantCulture)} kg"; // Mostrar con punto decimal

            return $"ID: {Id}, Nombre: {Nombre}, Apellidos: {Apellidos}, Email: {Email}, Estado: {Estado}{infoExtra}, Último Login: {LastLogin:dd/MM/yyyy HH:mm}"; // Ejemplo sin Suscripcion
        }

    }
}
