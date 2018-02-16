﻿using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Photon.MonoBehaviour
{
    #region To be removed

    public readonly PhotonPlayer PhotonPlayer;
    public int Health;

    #endregion To be removed

    #region Class Variables

    // Effects the player is currently under
    private List<Effect> _effects;

    private List<Effect> _expiredEffects;

    // Effects applied to other players when attacking them
    private List<Effect> _onHitEffects;

    // Public Stats
    [SerializeField]
    public float WalkSpeed = 10f;

    [SerializeField]
    public float JumpPower = 10f;

    // Private stats (access variables below)
    [SerializeField]
    private float _maxHp = 100f;

    private float _currentHp;

    [SerializeField]
    private float _baseDmg = 10f;

    // Damage modifiers
    public float dmgMult = 1f;

    public float dmgAdd = 0f;

    #endregion Class Variables

    #region Access Variables

    public float MaxHp { get { return _maxHp; } }
    public float CurrentHp { get { return _currentHp; } }
    public float BaseDamage { get { return _baseDmg; } }
    public float EffectiveDamage { get { return _baseDmg * dmgMult + dmgAdd; } } // Calculate effective damage with dmg mods
    public List<Effect> OnHitEffects { get { return _onHitEffects; } }

    #endregion Access Variables

    #region Unity Callbacks

    // Use this for initialization
    private void Start()
    {
        _effects = new List<Effect>();
        _expiredEffects = new List<Effect>();
        _onHitEffects = new List<Effect>();

        _currentHp = _maxHp;
    }

    // Update is called once per frame
    private void Update()
    {
        // Clean up the expired effects
        for (int i = 0; i < _expiredEffects.Count; ++i)
        {
            _effects.Remove(_expiredEffects[i]);
        }
        _expiredEffects.Clear();

        // Triggers each effect
        foreach (Effect e in _effects)
        {
            e.OnFrame();
        }

        // TEST
        if (Input.GetKeyDown("q"))
        {
            TakeDamage(10f);
        }
    }

    #endregion Unity Callbacks

    #region Public Methods

    // Add an effect to the player
    public void AddEffect(Effect effect)
    {
        if (effect.Unique == true)
        {
            bool found = false;
            foreach (Effect e in _effects)
            {
                if (e.Name == effect.Name && effect.Name != "")
                {
                    Debug.LogError("Cannot add effect. Unique effect already exists on player");
                    found = true;
                }
            }
            if (!found)
            {
                _effects.Add(effect);
            }
        }
        else
        {
            _effects.Add(effect);
            effect.Owner = gameObject;
            effect.Activate();
        }
    }

    // Called by effects when they expire
    public void RemoveEffect(Effect effect)
    {
        _expiredEffects.Add(effect);
    }

    // Add an OnHit effect to the player
    public void AddOnHit(Effect effect)
    {
        if (effect.Unique == true)
        {
            bool found = false;
            foreach (Effect e in _onHitEffects)
            {
                if (e.GetType() == effect.GetType())
                {
                    Debug.LogError("Cannot add effect. Unique effect already exists on player");
                    found = true;
                }
            }
            if (!found)
            {
                _onHitEffects.Add(effect);
            }
        }
        else
        {
            _onHitEffects.Add(effect);
            // Do NOT activate the effect or set an owner
        }
    }

    // Remove an OnHit effect from the player
    public void RemoveOnHit(Effect effect)
    {
        _onHitEffects.Remove(effect);
    }

    /// <summary>
    /// Cause the player to take damage
    /// </summary>
    /// <param name="amount">Amount of damage player will recieve</param>
    public void TakeDamage(float amount)
    {
        photonView.RPC("RPC_TakeDamage", PhotonTargets.All, amount, null, null);
    }

    /// <summary>
    /// Cause the player to take damage from a source
    /// </summary>
    /// <param name="source">Source damaging the player, can be null</param>
    /// <param name="amount">Amount of damage player will recieve</param>
    public void TakeDamage(float amount, GameObject source)
    {
        photonView.RPC("RPC_TakeDamage", PhotonTargets.All, amount, source, null);
    }

    /// <summary>
    /// Cause the player to take damage with effects
    /// </summary>
    /// <param name="amount">Amount of damage player will recieve</param>
    /// <param name="effects">Effects applied to the player, can be null</param>
    public void TakeDamage(float amount, List<Effect> effects)
    {
        photonView.RPC("RPC_TakeDamage", PhotonTargets.All, amount, null, effects);
    }

    /// <summary>
    /// Cause the player to take damage from a source with status effects
    /// </summary>
    /// <param name="source">Source damaging the player, can be null</param>
    /// <param name="amount">Amount of damage player will recieve</param>
    /// <param name="effects">Effects applied to the player, can be null</param>
    public void TakeDamage(float amount, GameObject source, List<Effect> effects)
    {
        photonView.RPC("RPC_TakeDamage", PhotonTargets.All, amount, source, effects);
    }

    // Increase the player's current hp by amount
    public void GainHp(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot gain negative hp. Use TakeDamage instead.");
            return;
        }
        if (_currentHp + amount > _maxHp)
        {
            Debug.LogWarning("Cannot overheal. Hp is max.");
            _currentHp = _maxHp;
        }
        else
        {
            _currentHp += amount;
        }
    }

    // Can be used later for checking accuracy etc
    public void ReportHit(GameObject hit)
    {
        Debug.Log(hit.name + " was hit by " + gameObject.name);
    }

    #endregion Public Methods

    #region Photon RPCs

    [PunRPC]
    private void RPC_TakeDamage(float amount, GameObject source, List<Effect> effects)
    {
        PlayerStats pSource = source.GetComponent<PlayerStats>();
        pSource.ReportHit(gameObject);

        if (effects != null)
        {
            // Add effects to player
            foreach (Effect e in effects)
            {
                e.ApplyEffect(gameObject);
            }
        }
        if (amount < 0)
        {
            Debug.LogWarning("Cannot take negative damage.");
            return;
        }
        // Reduce hp by amount
        _currentHp -= amount;
        if (_currentHp <= 0)
        {
            Debug.Log(gameObject.name + " hp <= 0");
            Die(source);
        }
    }

    #endregion Photon RPCs

    #region Private Methods

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");

        // No death logic yet
        _currentHp = _maxHp; // Resets hp
        Debug.LogWarning("Death logic not implemented yet. Player healed to full.");
    }

    private void Die(GameObject killer)
    {
        if (killer != null)
        {
            Debug.Log(gameObject.name + " was killed by " + killer.name);
        }
        else
        {
            Debug.Log(gameObject.name + " has died of mysterious causes.");
        }

        // No death logic yet
        _currentHp = _maxHp; // Resets hp
        Debug.LogWarning("Death logic not implemented yet. Player healed to full.");
    }

    #endregion Private Methods
}