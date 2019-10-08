using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class CheckpointManager : MonoBehaviour
    {
        public List<GameObject> checkpoints = new List<GameObject>();

        private PlayerMovement _player;

        void Start()
        {
            _player = GameObject.Find("Player").GetComponent<PlayerMovement>() ?? throw new Exception();
        }

        void Update()
        {
            if (!checkpoints.Any())
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (GameObject.Find("Player").GetComponent<PlayerCollisionChecker>().Zooming)
                {
                    // Don't unnecessarily fade out during a previous animation
                    return;
                }

                var fadeInOut = GameObject.Find("FadeInOut").GetComponent<FadeInOut>();
                fadeInOut.FadeOut(() =>
                {
                    // Only fade in after respawn
                    fadeInOut.skipNextFadeOut = true;

                    _player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    _player.transform.rotation = Quaternion.identity;
                    _player.transform.position = checkpoints.Last().transform.position;
                });
            }
        }

        public bool AddCheckpoint(GameObject checkpoint)
        {
            // Put it on top
            var alreadySaved = checkpoints.Contains(checkpoint);
            if (alreadySaved)
            {
                checkpoints.Remove(checkpoint);
            }

            checkpoints.Add(checkpoint);

            var light = checkpoint.transform.Find("Light").GetComponent<LightScript>();
            light.ChargeLight(light.maximumIntensity);
            light.lightDec = 0;
            return alreadySaved;
        }
    }
}
