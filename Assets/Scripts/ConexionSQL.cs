using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SqlClient;

public class ConexionSQL : MonoBehaviour
{
    private string connectionString = "";
    public List<string> Preguntas = new List<string>();
    public List<string> Respuestas = new List<string>();

    void Awake()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "SELECT * FROM dbo.Diccionario";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Obteniendo valores de columnas de tipo cadena (string)
                            string columna2 = reader.GetString(reader.GetOrdinal("Pregunta"));
                            string columna1 = reader.GetString(reader.GetOrdinal("Respuesta"));

                            Respuestas.Add(columna1);
                            Preguntas.Add(columna2);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Debug.LogError($"Error al conectar a la base de datos: {ex.Message}");
            }
        }
    }

    void Update()
    {
        
    }
}
