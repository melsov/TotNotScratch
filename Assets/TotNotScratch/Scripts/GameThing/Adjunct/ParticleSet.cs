using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GTParticleSet
{
    private Transform target;

    private static string particleFolderResources = "Particles";

    private Dictionary<string, ParticleSystem> _particles;
    private Dictionary<string, ParticleSystem> particles {
        get {
            if(_particles == null) { _particles = new Dictionary<string, ParticleSystem>(); } return _particles;
        }
    }

    public GTParticleSet(Transform target) {
        this.target = target;
    }

    public ParticleSystem getOrAddParticles(string prefabParticleName, Vector3 localOffset = default(Vector3)) {
        if(particles.ContainsKey(prefabParticleName)) { return particles[prefabParticleName]; }

        ParticleSystem ps = UnityEngine.Object.Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>(string.Format("{0}/{1}", particleFolderResources, prefabParticleName)));
        if(!ps) { throw new Exception(prefabParticleName + "not found in Resources/Particles"); }

        ps.transform.position = target.position + localOffset;
        ps.transform.SetParent(target);
        
        particles.Add(prefabParticleName, ps);
        return ps;
    }

    public void destroyParticles(string prefabParticleName) {
        if(!particles.ContainsKey(prefabParticleName)) { return; }

        UnityEngine.Object.Destroy(particles[prefabParticleName].gameObject);
    }

    public void play(string prefabParticleName) {
        ParticleSystem ps = getOrAddParticles(prefabParticleName);
        ps.Play();
    } 

    public void stop(string prefabParticleName) {
        if(!particles.ContainsKey(prefabParticleName)) { return; }
        particles[prefabParticleName].Stop();
    }
}
