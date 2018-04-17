﻿using UnityEngine;

namespace Powerups
{
    /// <summary>
    /// <para>Author: Colby Prince</para>
    /// <para>Power-Up Name: Growth</para>
    /// <para>Power-Up Category: Passive</para>
    /// <para>Power-Up Effect: Causes player to grow to a larger size, gaining a damage advantage and footfall AoE effect. Disadvantage would be easier to hit</para>
    /// <para>Power-Up Tier: Uncommon</para>
    /// <para>Power-Up Description:  Player model is larger,  gaining damage boost but same walk speed.</para>
    /// </summary>
    
    [RequireComponent(typeof(AudioSource))]
    public class Passive_Growth : PassiveAbility
    {
        public AudioClip FootFall;
        public float GrowthFactor = 2;
        public float DeadZone = .1f;
        

        private AudioManager AudioManager;
        private AudioSource AudioSource;
        private PlayerStats pStats;

        public float dmgMult = .5f;
        public float dmgAdd = 0f;

        public float LastFootfall = 0;

        private void Awake()
        {
            _name = "Growth";
            FootFall = Resources.Load<AudioClip>("Audio/Growth_FootFall");
            Icon = Resources.Load<Sprite>("Images/Growth");
            Tier = PowerupTier.Uncommon;
        }

        public override void OnAbilityAdd()
        {
            pStats = gameObject.GetComponent<PlayerStats>();
            AudioManager = GetComponent<AudioManager>();
            AudioSource = AudioManager.GetNewAudioSource(_name);
            // Only adjust scale on the controlling client because AddScaleFactor is networked
            if (photonView.isMine)
            {
                pStats.AddScaleFactor(_name, GrowthFactor);
                //pStats.AddDmgMultiplier(Name, dmgMult);
                //pStats.AddDmgBoost(Name, dmgAdd);
            }
            base.OnAbilityAdd();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //This whole thing is something I'm not sure we'll be using
            ////If moving
            //if (Mathf.Abs(Input.GetAxis("Horizontal")) > DeadZone || Mathf.Abs(Input.GetAxis("Vertical")) > DeadZone)
            //{
            //    //If not playing
            //    if (!AudioSource.isPlaying)
            //    {
            //        AudioSource.Play();
            //    }
            //}
            ////If standing still
            //else
            //{
            //    //If playing
            //    if (AudioSource.isPlaying)
            //    {
            //        AudioSource.Stop();
            //    }
            //}
        }
        
        public override void OnAbilityRemove()
        {
            pStats.RemoveDmgMultiplier(_name);
            pStats.RemoveDmgBoost(_name);
            // Only adjust scale on the controlling client because RemoveScaleFactor is networked
            if(photonView.isMine)
                pStats.RemoveScaleFactor(_name);
            AudioManager.DeleteAudioSource(_name);
            base.OnAbilityRemove();
        }
    }
}