﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Powerups;

public class Pickup : Photon.MonoBehaviour
{
    private AbilityManager aMan;
    private BaseAbility _ability;
    public GameObject spawner;
    //public List<BaseAbility> powerList;
    public string[] powerList;
    public List<string> AvailablePowerupStrings;
    // Use this for initialization
    //private void Start()
    //{
    //    //_ability = GetComponent<BaseAbility>();
    //    ////aMan = GameObject.FindGameObjectWithTag("Player").GetComponent<AbilityManager>();
    //    ////List<KeyValuePair<string, BaseAbility>> list = aMan.AbilityList.ToList();
    //    ////foreach(KeyValuePair<string,BaseAbility> power in list)
    //    ////{
    //    ////    Debug.Log(power);
    //    ////}
    //    //List<string> AvailablePowerupStrings = new List<string>();
    //    //Type[] Types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
    //    //Dictionary<string, Type> AbilityDict = new Dictionary<string, Type>();
    //    //foreach (Type type in Types)
    //    //{
    //    //    if (type.Namespace == "Powerups" && type.IsSubclassOf(typeof(BaseAbility)))
    //    //    {
    //    //        AvailablePowerupStrings.Add(type.Name);
    //    //        AbilityDict.Add(type.Name, type);
    //    //    }
    //    //}

    //}

    private void Awake()
    {
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC("AddPickupAbility", PhotonTargets.AllViaServer, null);
        }
        else return;
        
    }

    
    private void OnTriggerEnter(Collider other)
    {
        
        PhotonView pv = PhotonView.Get(this);
        _ability = GetComponent<BaseAbility>();
        if (other.gameObject.tag == "Player")
        {
            
            AbilityManager aManager = other.GetComponent<AbilityManager>();

            aManager.AddAbility(_ability);
            PowerupSpawner pSpawn = spawner.GetComponent<PowerupSpawner>();
            pSpawn.hasPickup = false;
            PhotonNetwork.Destroy(gameObject);
            if (photonView.isMine)
                PhotonNetwork.Destroy(photonView);


        }
    }

    [PunRPC]
    public void AddPickupAbility()
    {

        if (gameObject != null)
        {
            var rnd = new System.Random();

            AvailablePowerupStrings = new List<string>();
            Type[] Types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            Dictionary<string, Type> AbilityDict = new Dictionary<string, Type>();
            foreach (Type type in Types)
            {
                if (type.Namespace == "Powerups" && type.IsSubclassOf(typeof(BaseAbility)))
                {
                    AvailablePowerupStrings.Add(type.Name);
                    // AbilityDict.Add(type.Name, type);
                }
            }
            //Powers = AvailablePowerupStrings;
            string powerName = AvailablePowerupStrings.OrderBy(s => Guid.NewGuid()).First();
            Debug.Log(powerName);
            Type thisType = ReflectionUtil.GetAbilityTypeFromName(powerName);
            gameObject.AddComponent(thisType);
            //UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(this.gameObject,"Assets/Scripts/Powerups" ,powerName);

        }
    }

    [PunRPC]
    public void AbilityPickup()
    {
      
    }
  
}


