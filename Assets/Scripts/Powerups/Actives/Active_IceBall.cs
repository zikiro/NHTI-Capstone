﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Powerups
{
    public class Active_IceBall : ActiveAbility
    {
        private PlayerShoot pShoot;
        private Ice iceball;

        public readonly Vector3 PosOffset = new Vector3(0, 2, 0);

        public readonly Vector3 RotOffset = new Vector3(0, 0, 0);

        private void Awake()
        {
            Name = "Ice Ball";
            Icon = Resources.Load<Sprite>("Images/Ice Ball");
            Tier = PowerupTier.Common;
        }

        public override void OnAbilityAdd()
        {
            Debug.Log(Name + " Added");

            // Call base function
            base.OnAbilityAdd();
        }

        public override void OnAbilityRemove()
        {
            // Call base function
            base.OnAbilityRemove();
        }

        protected override void Activate()
        {
            if (photonView.isMine)
            {
                GameObject _proj = PhotonNetwork.Instantiate("Iceball", transform.position + PosOffset, Quaternion.LookRotation(transform.rotation.eulerAngles + RotOffset), 0);
            }
            base.Activate();
        }
    }
}