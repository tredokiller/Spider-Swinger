using System;
using Data.Player.Scripts.Movement.AnimationController;
using Data.Player.Scripts.Movement.Controller;
using UnityEngine;

namespace Data.Player.Scripts.SoundPlayer
{
    public class PlayerSoundController : MonoBehaviour
    {
        private AudioSource _audioSource;

        [SerializeField] private AudioClip webShooterSoundClip;
        [SerializeField] private AudioClip trickSoundClip;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            PlayerController.StartSwingingAction += () => PlaySound(webShooterSoundClip);
            PlayerController.StartAirMovement += () => PlaySound(trickSoundClip);
        }
        
        private void PlaySound(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
        
        private void OnDisable()
        {
            PlayerController.StartSwingingAction -= () => PlaySound(webShooterSoundClip);
        }
    }
}
