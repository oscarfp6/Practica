// Importa utilidades propias (Email, Password, Encriptar) y del sistema.
using MiLogica.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization; // Para formateo de números (ej. ToString)
using System.Linq;
using System.Runtime.CompilerServices; // Para InternalsVisibleTo
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MiLogica.ModeloDatos
{
    /// <summary>
    /// Representa a un usuario del sistema.
    /// Esta clase es responsable de gestionar sus propios datos (validación)
    /// y su estado de seguridad (lógica de bloqueo, inactividad, etc.).
    /// </summary>
    public class Usuario
    {
        // --- Constantes de Reglas de Negocio (Seguridad) ---
        // Definir estas reglas como constantes hace que sean fáciles de
        // entender y modificar en un solo lugar.

        /// <summary>
        /// Número máximo de intentos de contraseña fallidos antes de bloquear la cuenta.
        /// </summary>
        private const int MAX_INTENTOS_FALLIDOS = 3;

        /// <summary>
        /// Ventana de tiempo (en minutos) en la que se cuentan los intentos fallidos.
        /// </summary>
        private const int MINUTOS_VENTANA_INTENTOS = 5;

        /// <summary>
        /// Días que deben pasar sin un login exitoso para que la cuenta se marque como Inactiva.
        /// </summary>
        private const int DIAS_PARA_INACTIVIDAD = 182; // Aprox. 6 meses

        // --- Backing Fields (Campos de Respaldo) ---
        // Campos privados que almacenan la información. Se usan 'setters' públicos
        // para validar los datos antes de asignarlos a estos campos.
        public string _nombre;
        public string _apellidos;
        public string _email;
        public string _passwordHash; // Almacena la contraseña *encriptada*.
        public int? _edad;     // Nullable<int>: La edad es opcional.
        public double? _peso; // Nullable<double>: El peso es opcional.

        /// <summary>
        /// Almacena la fecha y hora hasta la cual la cuenta está bloqueada.
        /// Si es 'null' o la fecha es pasada, el bloqueo de "cooldown" no aplica.
        /// </summary>
        public DateTime? BloqueadoHasta { get; set; }

        /// <summary>
        /// Identificador único (Clave Primaria) del usuario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Peso del usuario en kg (opcional).
        /// </summary>
        public double? Peso
        {
            get => _peso;
            set
            {
                // VALIDACIÓN: Se ejecuta al asignar 'usuario.Peso = valor'.
                // Solo valida si el valor NO es null.
                if (value.HasValue && (value < 0 || value > 500))
                {
                    throw new ArgumentException("El peso debe estar entre 0 y 500 kg.");
                }
                _peso = value; // Permite asignar 'null' o un valor válido.
            }
        }

        /// <summary>
        /// Edad del usuario en años (opcional).
        /// </summary>
        public int? Edad
        {
            get => _edad;
            set
            {
                // VALIDACIÓN:
                if (value.HasValue && (value < 0 || value > 120))
                {
                    throw new ArgumentException("La edad debe estar entre 0 y 120 años.");
                }
                _edad = value;
            }
        }

        /// <summary>
        /// Nombre de pila del usuario.
        /// </summary>
        public string Nombre
        {
            get => _nombre;
            set
            {
                // VALIDACIÓN: No puede ser nulo, vacío o espacios en blanco.
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El nombre no puede estar vacío.");
                }
                // VALIDACIÓN: Usa una utilidad para chequear que no contenga números.
                if (!Utils.Valid.Nombre(value))
                {
                    throw new ArgumentException("El nombre no puede contener números u otros caracteres no válidos.");
                }
                _nombre = value;
            }
        }

        /// <summary>
        /// Apellidos del usuario.
        /// </summary>
        public string Apellidos
        {
            get => _apellidos;
            set
            {
                // VALIDACIÓN: (Idéntica a la de Nombre)
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

        /// <summary>
        /// Estado de la suscripción de pago (ej. Premium).
        /// </summary>
        public bool Suscripcion { get; set; }

        /// <summary>
        /// Email del usuario (usado como login).
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                // VALIDACIÓN: Usa la utilidad de validación de Email.
                if (!Utils.Email.ValidarEmail(value))
                {
                    throw new ArgumentException("El formato del email no es válido.");
                }
                _email = value;
            }
        }

        /// <summary>
        /// Fecha y hora del último inicio de sesión exitoso.
        /// </summary>
        public DateTime LastLogin { get; set; }

        /// <summary>
        /// Estado actual de la cuenta (Activo, Inactivo, Bloqueado).
        /// </summary>
        public EstadoUsuario Estado { get; set; }

        /// <summary>
        /// Lista que almacena las marcas de tiempo (timestamps) de los
        /// intentos de login fallidos recientes.
        /// </summary>
        public List<DateTime> intentosFallidosTimestamps;

        /// <summary>
        /// Constructor por defecto.
        /// Inicializa el objeto con valores seguros para evitar 'null reference'.
        /// Es útil para frameworks como Entity Framework o para crear un objeto
        /// antes de asignarle todos sus valores.
        /// </summary>
        public Usuario()
        {
            this.Email = "example@gmail.com"; // Email válido por defecto
            this.Nombre = "nameToChange";
            this.Apellidos = "surnamesToChange";
            this._passwordHash = string.Empty;
            _edad = null;
            _peso = null;
            this.intentosFallidosTimestamps = new List<DateTime>(); // ¡Importante inicializar la lista!
        }

        /// <summary>
        /// Constructor principal para crear un nuevo usuario.
        /// Valida el email y la contraseña en el momento de la creación.
        /// </summary>
        public Usuario(int id, string nombre, string password, string apellidos, string email, bool suscripcion)
        {
            // VALIDACIÓN EN CONSTRUCTOR:
            // Asegura que un usuario *nunca* pueda ser creado con un email
            // o contraseña inválidos desde el principio.
            if (!Utils.Email.ValidarEmail(email))
            {
                throw new ArgumentException("El formato del email no es valido");
            }
            if (!Utils.Password.ValidarPassword(password))
            {
                throw new ArgumentException("La contraseña no cumple los requisitos de seguridad.");
            }

            // Asignación de propiedades
            this.Id = id;
            this.Nombre = nombre; // Usa el 'setter' con validación
            this.Apellidos = apellidos; // Usa el 'setter' con validación
            this.Email = email;       // Usa el 'setter' con validación
            this.Suscripcion = suscripcion;
            _edad = null; // Opcionales se inician a null
            _peso = null;

            // Encriptación de la contraseña
            this._passwordHash = Encriptar.EncriptarPasswordSHA256(password);

            // --- Estado Inicial ---
            this.Estado = EstadoUsuario.Activo;
            this.intentosFallidosTimestamps = new List<DateTime>(); // Inicializa la lista de intentos
            this.LastLogin = DateTime.Now;
        }

        /// <summary>
        /// Lógica principal de autenticación y seguridad.
        /// Comprueba una contraseña y actualiza el estado (Activo, Bloqueado).
        /// </summary>
        /// <param name="passwordDado">La contraseña en texto plano introducida por el usuario.</param>
        /// <returns>True si el login es exitoso, False si falla.</returns>
        public bool PermitirLogin(string passwordDado)
        {
            // 1. Comprobar si el usuario está inactivo (por tiempo)
            VerificarInactividad();

            // 2. Comprobar si está en "cooldown" (bloqueo temporal)
            if (this.Estado == EstadoUsuario.Bloqueado && this.BloqueadoHasta.HasValue && DateTime.Now < this.BloqueadoHasta)
            {
                return false; // Cooldown activo, no se permite ni intentar.
            }

            // 3. Encriptar la contraseña dada para compararla
            string passwordEncriptada = Encriptar.EncriptarPasswordSHA256(passwordDado);
            bool esPasswordCorrecta = this._passwordHash.Equals(passwordEncriptada);
            bool estabaInactivo = (this.Estado == EstadoUsuario.Inactivo);

            // 4. --- CAMINO FELIZ (Contraseña Correcta) ---
            if (esPasswordCorrecta)
            {
                // Si estaba bloqueado PERO el cooldown ha expirado (ej. esperó 2 min),
                // el login falla. Debe usar el botón "Desbloquear".
                if (this.Estado == EstadoUsuario.Bloqueado)
                {
                    return false;
                }

                // Éxito: Limpia intentos fallidos, marca como Activo y actualiza LastLogin.
                this.intentosFallidosTimestamps.Clear();
                Estado = EstadoUsuario.Activo;
                this.LastLogin = DateTime.Now;
                return true;
            }
            // 5. --- CAMINO TRISTE (Contraseña Incorrecta) ---
            else
            {
                // REGLA DE NEGOCIO: Si estaba Inactivo, un solo fallo lo Bloquea.
                if (estabaInactivo)
                {
                    this.Estado = EstadoUsuario.Bloqueado;
                    this.intentosFallidosTimestamps.Clear();
                    return false; // Login falla
                }

                // Es un usuario Activo que ha fallado:
                // A. Añadir el intento fallido actual.
                this.intentosFallidosTimestamps.Add(DateTime.Now);

                // B. Limpiar intentos fallidos que estén fuera de la ventana de 5 minutos.
                var now = DateTime.Now;
                this.intentosFallidosTimestamps = this.intentosFallidosTimestamps
                    .Where(t => (now - t).TotalMinutes <= MINUTOS_VENTANA_INTENTOS)
                    .ToList();

                // C. Comprobar si ha superado el máximo de intentos.
                if (this.intentosFallidosTimestamps.Count >= MAX_INTENTOS_FALLIDOS)
                {
                    this.Estado = EstadoUsuario.Bloqueado;
                    this.BloqueadoHasta = DateTime.Now.AddMinutes(2); // Inicia el cooldown
                }
            }
            return false; // Login falla
        }

        /// <summary>
        /// Compara una contraseña en texto plano con el hash almacenado.
        /// Es un método "puro", no cambia el estado del usuario.
        /// </summary>
        public bool ComprobarPassWord(string passwordAComprobar)
        {
            string passwordEncriptada = Encriptar.EncriptarPasswordSHA256(passwordAComprobar);
            return this._passwordHash == passwordEncriptada;
        }


        /// <summary>
        /// Desbloquea una cuenta que estaba en estado Bloqueado o Inactivo,
        /// siempre que el "cooldown" haya expirado y la contraseña sea correcta.
        /// </summary>
        public bool DesbloquearUsuario(string email, string passwordDado)
        {
            // 1. Si el cooldown sigue activo, no se puede desbloquear.
            if (this.Estado == EstadoUsuario.Bloqueado && DateTime.Now < this.BloqueadoHasta)
            {
                return false;
            }

            // 2. Condiciones para desbloquear:
            //    - El email debe coincidir (ignorando mayúsculas).
            //    - El estado debe ser Bloqueado (con cooldown expirado) O Inactivo.
            //    - La contraseña debe ser correcta.
            if (this.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                (this.Estado == EstadoUsuario.Bloqueado || this.Estado == EstadoUsuario.Inactivo) &&
                ComprobarPassWord(passwordDado))
            {
                RestablecerCuenta(); // Pone el estado en Activo y limpia intentos/bloqueo.
                return true;
            }

            // 3. Si no se cumplen las condiciones, el desbloqueo falla.
            return false;
        }


        /// <summary>
        /// Comprueba si el usuario ha estado inactivo demasiado tiempo y,
        /// si es así, cambia su estado a Inactivo.
        /// </summary>
        public void VerificarInactividad()
        {
            if (this.Estado == EstadoUsuario.Activo)
            {
                TimeSpan tiempoSinAcceder = DateTime.Now - this.LastLogin;
                if (tiempoSinAcceder > TimeSpan.FromDays(DIAS_PARA_INACTIVIDAD))
                {
                    this.Estado = EstadoUsuario.Inactivo;
                }
            }
        }

        /// <summary>
        /// Permite a un usuario cambiar su propia contraseña.
        /// </summary>
        /// <param name="passwordActual">La contraseña antigua (para verificación).</param>
        /// <param name="nuevoPassword">La nueva contraseña (se validará).</param>
        /// <returns>True si el cambio fue exitoso.</returns>
        public bool CambiarPassword(string passwordActual, string nuevoPassword)
        {
            // 1. REGLA DE NEGOCIO: No se puede cambiar si está bloqueado.
            if (this.Estado == EstadoUsuario.Bloqueado)
            {
                return false;
            }

            // 2. VALIDACIÓN:
            //    - La contraseña actual debe ser correcta.
            //    - La nueva contraseña debe cumplir las reglas de seguridad.
            if (!ComprobarPassWord(passwordActual) || !Utils.Password.ValidarPassword(nuevoPassword))
            {
                return false;
            }

            // 3. Asignar el nuevo hash y restablecer la cuenta (por seguridad).
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

        /// <summary>
        /// Método auxiliar privado para centralizar el reseteo de la cuenta a un
        /// estado "limpio" y "Activo".
        /// </summary>
        private void RestablecerCuenta()
        {
            this.Estado = EstadoUsuario.Activo;
            this.intentosFallidosTimestamps.Clear(); // Limpia intentos fallidos
            this.LastLogin = DateTime.Now;
            this.BloqueadoHasta = null; // Quita cualquier bloqueo
        }

        /// <summary>
        /// Actualiza los datos del perfil del usuario (Nombre, Apellidos, Edad, Peso).
        /// </summary>
        public void ActualizarPerfil(string nombre, string apellidos, int? edad, double? peso)
        {
            // Al asignar a las propiedades (this.Nombre), se ejecutan
            // automáticamente los 'setters' de validación.
            this.Nombre = nombre;
            this.Apellidos = apellidos;
            this.Edad = edad;
            this.Peso = peso;
        }

        /// <summary>
        /// Sobrescribe el método ToString() para devolver una representación
        /// legible del usuario, útil para depuración o logs.
        /// </summary>
        public override string ToString()
        {
            string infoExtra = "";
            if (Edad.HasValue) infoExtra += $", Edad: {Edad.Value}";
            if (Peso.HasValue) infoExtra += $", Peso: {Peso.Value.ToString(CultureInfo.InvariantCulture)} kg";

            return $"ID: {Id}, Nombre: {Nombre}, Apellidos: {Apellidos}, Email: {Email}, Estado: {Estado}{infoExtra}, Último Login: {LastLogin:dd/MM/yyyy HH:mm}";
        }

    }
}
