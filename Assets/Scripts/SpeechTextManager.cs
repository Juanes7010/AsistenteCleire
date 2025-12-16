using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextSpeech;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;
using OpenAI;

public class SpeechTextManager : MonoBehaviour
{
    [SerializeField] string language = "es-ES";
    [SerializeField] TMP_Text uIText;
    [SerializeField] TMP_Text PruebaText;
    [SerializeField] string input;
    [SerializeField] Animator animator;
    [SerializeField] GameObject panelInst;
    [SerializeField] List<string> PalabrasClave;
    [SerializeField] List<GameObject> Imagenes;

    private OpenAIApi openai = new OpenAIApi("", "");
    private List<ChatMessage> messages = new List<ChatMessage>();
    private string prompt = "Actua como un vendedor y responde a las preguntas, vendes todo tipo de accesorios " +
        "para celular, nunca menciones que eres un modelo de ia y responde que solo estas capacitada para " +
        "responder sobre temas de accesorios para celular cuando te pregunten algo que no esté relacionado " +
        "con el tema, es importante que los precios siempre estén en pesos colombianos, " +
        "cuando te pregunten por stock o cantidad de productos que hay en existencia, debes responder siempre" +
        "no olvides responder que solo estas capacitada para responder sobre temas de accesorios para celular " +
        "cuando te pregunten algo que no esté relacionado con el tema y responde de manera corta y precisa";

    string PalabraClave;

    [Serializable]
    public struct VoiceCommand
    {
        public string Keyword;
        public UnityEvent Response;
    }

    public VoiceCommand[] voiceCommands;

    private Dictionary<string, UnityEvent> commands = new Dictionary<string, UnityEvent>();
    private void Awake()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
        foreach (var command in voiceCommands)
        {
            commands.Add(command.Keyword.ToLower(), command.Response);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TextToSpeech.Instance.Setting(language, 1, 1);
        SpeechToText.Instance.Setting(language);

        SpeechToText.Instance.onResultCallback = OnFinalSpeechResult;
        TextToSpeech.Instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.Instance.onDoneCallback = OnSpeakStop;
#if UNITY_ANDROID
        SpeechToText.Instance.onPartialResultsCallback = OnPartilSpeechResult;
#endif
    }

    public class Accesorio
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }

        public Accesorio(string producto, int cantidad, decimal precio)
        {
            Producto = producto;
            Cantidad = cantidad;
            Precio = precio;
        }
    }

    public void StartListening()
    {
        SpeechToText.Instance.StartRecording();
    }

    public void StopListening()
    {
        SpeechToText.Instance.StopRecording();
    }

    public void OnFinalSpeechResult(string result)
    {
        uIText.text = result;
        input = result.ToLower();
        if(result != null)
        {
            UnityEvent response = null;
            if(result.ToLower() == "activar asistente" || result.ToLower() == "desactivar asistente")
            {
                 response = commands[result.ToLower()];
            }
            else
            {
                if (!panelInst.activeInHierarchy)
                {
                    response = commands[" "];
                }
                else
                {
                    response = null;
                }
            }
            if(response != null)
            {
                response?.Invoke();
            }
        }
    }

    public void OnPartilSpeechResult(string result)
    {
        uIText.text = result;
    }

    public void StartSpeaking(string message)
    {
        animator.SetBool("Speaking", true);
        TextToSpeech.Instance.StartSpeak(message);
    }

    public void StopSpeaking()
    {
        animator.SetBool("Speaking", false);
        TextToSpeech.Instance.StopSpeak();
    }

    public void OnSpeakStart()
    {
        Debug.Log("Talking start...");
    }

    public void OnSpeakStop()
    {
        animator.SetBool("Speaking", false);
        Debug.Log("Talking stop...");
    }


    public async void Iniciar()
    {
        string arreglo = "";

        foreach (var item in PalabrasClave)
        {
            arreglo += $"{item} ";
        }

        ChatMessage newMessage1 = new ChatMessage();
        newMessage1.Role = "user";
        newMessage1.Content = $"¿La pregunta {input} puede relacionarse con alguna de las palabras clave escritas a continuacion?   Lista: {arreglo}.   Quiero que tu respuesta solo contenga la palabra clave a la que está relacionada la pregunta, no respondas nada más adicional a la palabra clave";

        CreateChatCompletionRequest request1 = new CreateChatCompletionRequest();
        request1.Messages = new List<ChatMessage>
        {
            new ChatMessage{ Content = newMessage1.Content, Role = newMessage1.Role}
        };
        request1.Model = "gpt-3.5-turbo-0125";

        var response1 = await openai.CreateChatCompletion(request1);

        if (response1.Choices != null && response1.Choices.Count > 0)
        {
            PalabraClave = response1.Choices[0].Message.Content;
        }

        if (PalabraClave == "Estuches")
        {
            Imagenes[0].gameObject.SetActive(true);
            Imagenes[1].gameObject.SetActive(false);
            Imagenes[2].gameObject.SetActive(false);
            Imagenes[3].gameObject.SetActive(false);
        } else if (PalabraClave == "Audifonos")
        {
            Imagenes[0].gameObject.SetActive(false);
            Imagenes[1].gameObject.SetActive(true);
            Imagenes[2].gameObject.SetActive(false);
            Imagenes[3].gameObject.SetActive(false);
        }
        else if (PalabraClave == "Cargadores")
        {
            Imagenes[0].gameObject.SetActive(false);
            Imagenes[1].gameObject.SetActive(false);
            Imagenes[2].gameObject.SetActive(true);
            Imagenes[3].gameObject.SetActive(false);
        }
        else if (PalabraClave == "Celular")
        {
            Imagenes[0].gameObject.SetActive(false);
            Imagenes[1].gameObject.SetActive(false);
            Imagenes[2].gameObject.SetActive(false);
            Imagenes[3].gameObject.SetActive(true);
        }
        else
        {
            Imagenes[0].gameObject.SetActive(false);
            Imagenes[1].gameObject.SetActive(false);
            Imagenes[2].gameObject.SetActive(false);
            Imagenes[3].gameObject.SetActive(false);
        }

        ChatMessage newMessage = new ChatMessage();
        if (messages.Count == 0)
        {
            newMessage.Content = prompt + input;
        }
        if (messages.Count > 0)
        {
            newMessage.Content = input;
        }
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo-0125";

        var response = await openai.CreateChatCompletion(request);

        PruebaText.text = "Se quedó";

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            PruebaText.text = chatResponse.Content;

            StartSpeaking(chatResponse.Content);

        }
    }

}
