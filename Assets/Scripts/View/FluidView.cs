using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using UnityEngine;
using Random = System.Random;

public class FluidView : MonoBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private MeshRenderer _pixel;
    [SerializeField] private int _size;
    [SerializeField] private int _diffusion;
    [SerializeField] private int _viscosity;
    [SerializeField] private int _iteration;
    [SerializeField] private float _deltaTime;
    [SerializeField] private Vector3 _velocityPower;
    [SerializeField] private float _densityPower;
    private int _dt;
    private List<Material> _pixels = new List<Material>();

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
        for (int i = -_size / 2; i < _size / 2; i++)
        {
            for (int j = -_size / 2; j < _size / 2; j++)
            {
                for (int k = -_size / 2; k < _size / 2; k++)
                {
                    var sp = Instantiate(_pixel, new Vector3(i, j, k) + transform.position, Quaternion.Euler(0,90,0));
                    _pixels.Add(sp.material);
                }
            }

            await Task.Yield();
        }

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
        for (int i = 0; i < _pixels.Count; i++)
        {
            var pixel = _pixels[i];
            pixel.color =
                new Color(_color.r, _color.g, _color.b, _cube.densities[i]);
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

    //  void Update()
    // {
    //     if(!_isReady) return;
    //
    //     if (Input.GetMouseButton(0))
    //     {
    //         AddDensity();
    //         AddVelocity();
    //     }
    //     _simulator.Step();
    //     ChangeColor();
    // }
}