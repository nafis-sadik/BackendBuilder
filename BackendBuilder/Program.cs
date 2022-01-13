using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace RunInCMD
{
    public class Engine
    {
        private string EntityName;
        private string ProjectName;
        public Engine(string entityName, string projectName)
        {
            EntityName = entityName;
            ProjectName = projectName;
        }

        public void GenerateFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (StreamReader reader = new StreamReader(sourceFile))
                {
                    using (StreamWriter writer = new StreamWriter(destinationFile))
                    {
                        while (reader.Peek() >= 0)
                        {
                            string line = reader.ReadLine();
                            if (line.Contains("{{EntityName}}"))
                                line = line.Replace("{{EntityName}}", EntityName);
                            if (line.Contains("{{ProjectName}}"))
                                line = line.Replace("{{ProjectName}}", ProjectName);

                            writer.WriteLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Exception Message: {0}", ex.Message);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string currentFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string workingDirectory = Path.GetDirectoryName(currentFilePath);
                string sourceFile = "";
                string destinationFile = "";

                // Take Input
                Console.Write("Project Name : ");
                string ProjectName = Console.ReadLine();
                Console.Write("Entity Name : ");
                string EntityName = Console.ReadLine();
                destinationFile += EntityName + ".cs";

                // Create Controller
                sourceFile = workingDirectory + @"\ReferenceFiles\Controllers\SampleController.txt";
                destinationFile = workingDirectory + @"\Output\Controllers\";
                destinationFile += EntityName + "Controller.cs";
                Engine program = new Engine(EntityName, ProjectName);
                program.GenerateFile(sourceFile, destinationFile);

                // Create Service Interfaces
                sourceFile = workingDirectory + @"\ReferenceFiles\Interfaces\ISampleService.txt";
                destinationFile = workingDirectory + @"\Output\Abstraction\";
                destinationFile += "I" + EntityName + "Service.cs";
                program.GenerateFile(sourceFile, destinationFile);

                // Create Service
                sourceFile = workingDirectory + @"\ReferenceFiles\Services\SampleService.txt";
                destinationFile = workingDirectory + @"\Output\Services\";
                destinationFile += EntityName + "Service.cs";
                program.GenerateFile(sourceFile, destinationFile);
            }
            catch (IOException iox)
            {
                Console.WriteLine("Handeled Exception");
                Console.WriteLine(iox.Message);
            }

            string queryString = "SELECT table_name FROM all_tables WHERE owner='UMS' ORDER BY table_name";
            Console.Write("Connection String : ");
            string connectionString = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@tPatSName", "Your-Parm-Value");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}, {1}", reader["tPatCulIntPatIDPk"], reader["tPatSFirstname"]));
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            Console.ReadLine();
        }
    }
}
