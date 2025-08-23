using System;
using UnityEngine;
using Yarn.Unity;

namespace Yarn
{
    /// <summary>
    /// Interface for Yarn to control particles.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleController : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        [YarnCommand("play_particles")]
        public void Play()
        {
            _particleSystem.Play();
        }

        [YarnCommand("stop_particles")]
        public void Stop()
        {
            _particleSystem.Stop();
        }
        
        [YarnCommand("set_particle_velocity")]
        public void SetParticleVelocities(float xVel, float yVel)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
            int particleCount = _particleSystem.GetParticles(particles);

            for (int i = 0; i < particleCount; i++)
                particles[i].velocity = new Vector3(xVel, yVel);

            _particleSystem.SetParticles(particles, particleCount);
        }
    }
}
