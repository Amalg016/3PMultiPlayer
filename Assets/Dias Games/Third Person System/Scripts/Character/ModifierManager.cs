using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DiasGames.ThirdPersonSystem
{
    public class ModifierManager : MonoBehaviourPunCallbacks
    {
        public List<Modifier> m_Modifiers;

        private ThirdPersonSystem m_System;

        
        private void Awake()
        {
           
            if (photonView.IsMine)
            {
                m_System = GetComponent<ThirdPersonSystem>();

                // Initialize each modifier attached to character
                foreach (Modifier mod in m_Modifiers)
                    mod.Initialize(m_System);
            }
        }
        private void Update()
        {
            if (photonView.IsMine)
            {
                foreach (Modifier mod in m_Modifiers)
                    mod.UpdateModifier();
            }
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine)
            { return; }
                foreach (Modifier mod in m_Modifiers)
                    mod.FixedUpdateModifier();
            
        }

        public Modifier GetModifier<T>()
        {
            return m_Modifiers.Find(x => x is T);
        }
    }
}