using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using UnityEngine;

public class ParticleFluidView : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;

    [SerializeField] private Color _color;
    [SerializeField] private int _size;
    [SerializeField] private int _diffusion;
    [SerializeField] private int _viscosity;
    [SerializeField] private int _iteration;
    [SerializeField] private float _deltaTime;
    [SerializeField] private Vector3 _velocityPower;
    [SerializeField] private float _densityPower;
    private int _dt;

    private FluidCube _cube;
    private Simulator _simulator;
    private bool _isReady;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * _size);
    }
    async void Start()
    {
        _dt = (int)(_deltaTime * 1000);
        _cube = new FluidCube(_size, _diffusion, _viscosity, _deltaTime);
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        var numParticlesAlive  = _particleSystem.GetParticles(_particles);
        ChangeColor();
        _simulator = new Simulator(_iteration, _cube);
        _isReady = true;
        UpdateSimulate();
    }

    void AddVelocity()
    {
        _simulator.AddVelocity(8, 2, 8, _velocityPower.x, _velocityPower.y, _velocityPower.z);
    }

    void AddDensity()
    {
        _simulator.AddDensity(8, 2, 8, _densityPower);
    }

    async void ChangeColor()
    {
        for (int i = 0; i < _particles.Length; i++)
        {
            var particle = _particles[i];
            particle
            .color = new Color(_color.r, _color.g, _color.b, _cube.densities[i]);
        }
    }


    async void UpdateSimulate()
    {
        while (_isReady)
        {
            if (Input.GetMouseButton(0))
            {
                AddDensity();
                AddVelocity();
            }

            _simulator.Step();
            ChangeColor();
            await Task.Delay(20);
        }
    }

    private void OnApplicationQuit()
    {
        _isReady = false;
    }
}
