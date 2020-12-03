using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientAdmin
{
    class ProgramClientAdmin
    {
        private const int CREATE_USER = 1;
        private const int MODIFY_USER = 2;
        private const int DELETE_USUARIO = 3;
        private const int ALL_LOGS = 4;
        private const int SUCCESS_LOGS = 5;
        private const int WARNING_LOGS = 6;
        private const int EXIT = 7;
        private const string URI_USUARIO = "https://localhost:44358/Users";
        private const string URI_LOGS = "https://localhost:44381/Logs";
        private const string ERROR_REQUEST_MESSAGE = "No se pudo hacer la peticion";

        public static async Task Main(string[] args)
        {
            HttpClient http = new HttpClient();
            Console.WriteLine("Bienvenido Admin...... ");
            bool exit = false;
            int option;
            string uri = "";
            string username = "";
            while (!exit)
            {
                Menu();
                option = GetOption();
                switch (option)
                {
                    case CREATE_USER:
                        Console.WriteLine("Ingresar Usuario");
                        username = Console.ReadLine();
                        Console.WriteLine("Ingresar Contraseña");
                        string password = Console.ReadLine();
                        uri = $"{URI_USUARIO}?username={username}&password={password}";
                        await PostRequest(uri, http);
                        break;
                    case MODIFY_USER:
                        Console.WriteLine("Ingresar el usuario que quiere modificar");
                        username = Console.ReadLine();

                        Console.WriteLine("Ingresar el nuevo nombre de usuario");
                        string newUsername = Console.ReadLine();
                        Console.WriteLine("Ingresar la nueva contraseña");
                        string newPassword = Console.ReadLine();
                        uri = $"{URI_USUARIO}?username={username}&newPassword={newPassword}&newUsername={newUsername}";
                        await PutRequest(uri, http);
                        break;

                    case DELETE_USUARIO:
                        Console.WriteLine("Ingresar Usuario que se quiere eliminar");
                        username = Console.ReadLine();
                        uri = $"{URI_USUARIO}?username={username}";
                        await DeleteRequest(uri, http);
                        break;
                    case ALL_LOGS:
                        await GetRequest(URI_LOGS,http);
                        break;
                    case SUCCESS_LOGS:
                        uri = $"{URI_LOGS}/success";
                        await GetRequest(uri, http);
                        break;
                    case WARNING_LOGS:
                        uri = $"{URI_LOGS}/warning";
                        await GetRequest(uri, http);
                        break;
                    case EXIT:
                        exit = true;
                        http.Dispose();
                        Console.WriteLine("Exiting.....");
                        break;
                }
            }
        }

        private static async Task GetRequest(string uri, HttpClient http)
        {
            try
            {
                var response = await http.GetAsync(uri);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }catch (HttpRequestException)
            {
                Console.WriteLine(ERROR_REQUEST_MESSAGE);
            }
        }
        private static async Task PostRequest(string uri, HttpClient http)
        {
            try
            {
                var response = await http.PostAsync(uri, null);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException)
            {
                Console.WriteLine(ERROR_REQUEST_MESSAGE);
            }
        }
        private static async Task PutRequest(string uri, HttpClient http)
        {
            try
            {
                var response = await http.PutAsync(uri, null);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException)
            {
                Console.WriteLine(ERROR_REQUEST_MESSAGE);
            }
        }
        private static async Task DeleteRequest(string uri, HttpClient http)
        {
            try
            {
                var response = await http.DeleteAsync(uri);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException)
            {
                Console.WriteLine(ERROR_REQUEST_MESSAGE);
            }
        }

        private static void Menu()
        {
            Console.WriteLine(CREATE_USER + " - Crear Usuario\n");
            Console.WriteLine(MODIFY_USER+" - Modificar Usuario\n");
            Console.WriteLine(DELETE_USUARIO+" - Borrar Usuario\n");
            Console.WriteLine(ALL_LOGS + " - All logs\n");
            Console.WriteLine(SUCCESS_LOGS + " - Success logs\n");
            Console.WriteLine(WARNING_LOGS + " - Warning logs\n");
            Console.WriteLine(EXIT + " - Exit\n");
        }

        private static int GetOption()
        {
            int ret = Int32.MinValue;
            bool exit = false;
            while (!exit)
            {
                try
                {
                    int aux = Int32.Parse(Console.ReadLine());
                    if (IsValid(aux))
                    {
                        ret = aux;
                        exit = true;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Formato de opcion invalido. Debe ser un numero");
                }
            }
            return ret;
        }
        private static bool IsValid(int option)
        {
            return option <= EXIT && option >= CREATE_USER;
        }
    }
}
