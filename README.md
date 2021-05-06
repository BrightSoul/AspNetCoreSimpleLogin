# Simple password authentication
Quest'applicazione ASP.NET Core 5 dimostra come sia possibile autenticarsi anche senza usare ASP.NET Core Identity.

 * Il cookie di autenticazione viene emesso da `HttpContext.SignInAsync`;
 * I dati di autenticazione (username, hash della password e salt) sono memorizzati in configurazione nel file `appsettings.json` ma dovrebbero essere forniti su variabili d'ambiente, dato che sono dati sensibili;
 * L'hash della password è possibile generarlo con la classe `Rfc2898DeriveBytes` (vedere l'action `Login` nell'`HomeController` per capire come generarla.)

## Attenzione
Questa è solo una demo e **NON** si consiglia di usarla in produzione. È preferibile comunque ricorrere ad ASP.NET Core Identity perché impiega vari accorgimenti che rendono l'accesso degli utenti sicuro, come:
 * Blocco dell'account in caso in caso di tentantivo di accesso con la forza bruta;
 * Autenticazione a due fattori;
 * Hashing delle password e criteri di complessità.