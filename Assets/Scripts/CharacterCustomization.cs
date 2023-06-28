using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CharacterCustomization : MonoBehaviour
{
    //public static CharacterCustomization Instance;
  //  public GameObject[] prefabs;
  // [HideInInspector] public GameObject CharacterController;
  [HideInInspector] public string CharacterController;
    public TMP_Text CharacterName;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Character"))
        {
           CharacterController = PlayerPrefs.GetString("Character");
            OnCharacterChanged();
        }
        else
        {
            Normal();      
        }
    }

    public void OnCharacterChanged()
    {
      CharacterName.text = CharacterController;
    }

    public void Cowboy()
    {
        PlayerPrefs.SetString("Character","Cowboy");        
        CharacterController = "Cowboy";
        OnCharacterChanged();
    } 
    public void Normal()
    {
        PlayerPrefs.SetString("Character","Normal");
        CharacterController = "Normal";
        OnCharacterChanged();
    } 
    public void Police()
    {
        PlayerPrefs.SetString("Character","Police");
        CharacterController = "Police";
        OnCharacterChanged();
    } 
    public void Female()
    {
        PlayerPrefs.SetString("Character","Female");
        CharacterController = "Female";
        OnCharacterChanged();
    }
    public void Female2()
    {
        PlayerPrefs.SetString("Character","Female2");
        CharacterController = "Female2";
        OnCharacterChanged();
    }
}
