using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using LitJson;

public class Login : MonoBehaviour
{
  
    public InputField usernameText, passwordText;
    public Text messageText;

    private string username, password, regName, regUsername, regPass, regConfPass, regEmail;

    public void LogIn()
    {
        messageText.text = "";

        username = usernameText.text;
        password = passwordText.text;

        if (username == "" || password == "")
        {
            messageText.text = "Please complete all fields.";
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);
            PlayerPrefs.SetString("user", username);
            PlayerPrefs.SetString("pass", password);
            WWW w = new WWW("http://192.168.1.8/login", form);
            StartCoroutine(LogIn(w));
        }
    }
 

    
    private IEnumerator LogIn(WWW _w)
    {
        yield return _w;

        if (_w.error == null)
        {
            JsonData json = JsonMapper.ToObject(_w.text);
            if (json["status"].Equals(200))
            {
                Debug.Log("LoggedIn");
                SceneManager.LoadScene("Menu");
            }
            else
            {
                Debug.Log(json);
                messageText.text = json["message"].ToString();
            }
        }
        else
            messageText.text = "ERROR: " + _w.error;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Register() {
        Application.OpenURL("http://192.168.1.8/register");
    }
}