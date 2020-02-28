using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{


    public delegate void Interact(GameObject _Player);

    public Interact OnNearEnter;
    public Interact OnNearStay;
    public Interact OnNearExit;
    public Interact OnMidEnter;
    public Interact OnMidStay;
    public Interact OnMidExit;
    public Interact OnFarEnter;
    public Interact OnFarStay;
    public Interact OnFarExit;

}
