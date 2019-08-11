using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class GearMenu : MonoBehaviour {

	private string username;
	private string password;
	public static List<int> inventory = new List<int>();
    public static int itemSelected = 0;
    private bool getting = false;
	// Use this for initialization
	void Start () {
        itemSelected = 0;
		WWWForm form = new WWWForm();
		username = PlayerPrefs.GetString("user");
        password = PlayerPrefs.GetString("pass");
        form.AddField("username", username);
        form.AddField("password", password);
        WWW w = new WWW("http://192.168.1.8/login", form);
        StartCoroutine(LogIn(w));



        GameObject.Find("Button1").GetComponent<Button>().onClick.AddListener(() => {itemSelected = 1;});
        GameObject.Find("Button2").GetComponent<Button>().onClick.AddListener(() => {itemSelected = 2;});
        GameObject.Find("Button3").GetComponent<Button>().onClick.AddListener(() => {itemSelected = 3;});
        GameObject.Find("Button4").GetComponent<Button>().onClick.AddListener(() => {itemSelected = 4;});
	}
	
	// Update is called once per frame
	void Update () {

        int[] arr1 = new int[3] {0,0,0};

        foreach(int i in inventory){
            Debug.Log(i);
            arr1[i-2] = arr1[i-2] + 1;
        }

        for(int x = 1; x < 4; x++) {
            GameObject.Find("number" + (x+1)).GetComponent<Text>().text = arr1[x-1].ToString();
        }



        if(itemSelected != 0) {
            if(itemSelected == 1) {
                SceneManager.LoadScene("Level_2");
                return;
            }
            if(arr1[itemSelected-2] == 0) {
                itemSelected = 0;
                return;
            }
            Debug.Log("THIS ITEM: " + itemSelected);
            if(!getting){
                getting = true;
                WWWForm form2 = new WWWForm();
                username = PlayerPrefs.GetString("user");
                password = PlayerPrefs.GetString("pass");
                form2.AddField("username", username);
                form2.AddField("password", password);
                form2.AddField("item", (itemSelected));
                WWW w2 = new WWW("http://192.168.1.8/invrem", form2);
                StartCoroutine(invUpdate(w2));
            }
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
                inventory = new List<int>();
				for(int x = 0; x < json["inventory"].Count; x++){
					int i = int.Parse(json["inventory"][x].ToString());
					inventory.Add(i);
				}
            }
            else
            {
                Debug.Log(json);
            }
        }
        else
            Debug.Log("ERROR: " + _w.error);
    }

    private IEnumerator invUpdate(WWW _w)
    {
        yield return _w;

        if (_w.error == null)
        {
            JsonData json = JsonMapper.ToObject(_w.text);
            if (json["status"].Equals(200))
            {
                SceneManager.LoadScene("Level_2");
            }
            else
            {
                Debug.Log(json);
                getting = false;
            }
        }
        else
            Debug.Log("ERROR: " + _w.error);
            getting = false;
    }
}
