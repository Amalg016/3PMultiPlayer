using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Audio;
public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView PV;
    AudioSource Audio;
    AudioClip clip1;
    public  ParticleSystem muzzle;
    public override void Use()
    {
        Shoot();
    }
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Audio = GetComponentInParent<AudioSource>();
        clip1 = ((GunInfo)itemInfo).burst;
    }
    void Shoot()
    {
       // Ray ray = cam.ScreenPointToRay(new Vector3(0.5f, 0.5f));
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f,0f));
        ray.origin = cam.transform.position;
        ray.direction = cam.transform.forward;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            float damage=(((GunInfo)itemInfo).damage);
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage.ToString("00") , (PhotonNetwork.LocalPlayer.ActorNumber).ToString("00"));
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point,hit.normal);      
        }
      //  ShootSound();
    }
    
    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition,Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
           var impact=Instantiate(bulletImpactPrefab, hitPosition+hitNormal*0.001f, Quaternion.LookRotation(hitNormal,Vector3.up)*bulletImpactPrefab.transform.rotation);
            Destroy(impact.gameObject, 10f);
            impact.gameObject.transform.SetParent(colliders[0].transform);
        }
         ShootSound();
    }
    void ShootSound()
    {
        Audio.PlayOneShot(clip1);
        muzzle.Emit(1);
    }
}
