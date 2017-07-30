using UnityEngine;
using System.Collections;


namespace MalbersAnimations.HAP
{

    /// <summary>
    /// This Enable the mounting System
    /// </summary>
    public class MountTriggers : MonoBehaviour
    {
        public MountSide mountSide;
        IMount Montura;
        Rider rider;

        // Use this for initialization
        void Awake()
        {
            Montura = GetComponentInParent<IMount>(); //Get the Mountable in the parents
        }

        void OnTriggerEnter(Collider other)
        {
            GetAnimal(other);
        }

        void OnTriggerExit(Collider other)
        {
            rider = other.GetComponentInChildren<Rider>();

            if (rider != null)
            {
                if (rider.isRiding) return;                             //Hack when the Rider is mounting another horse and pass really close to a new horse

                if (rider.Mount_Side == mountSide && !Montura.Mounted)
                {
                    rider.CanMount = false;
                    rider.Montura = null;
                    rider.OnFindMount.Invoke(null);
                }
                rider = null;
            }
        }

        private void GetAnimal(Collider other)
        {

            if (!Montura.Mounted && Montura.CanBeMounted)      //If there's no other Rider on the Animal or the the Animal isn't death
            {
                rider = other.GetComponentInChildren<Rider>();
                if (rider != null)
                {
                    if (rider.isRiding) return;

                    rider.CanMount = true;
                    rider.Montura = Montura;
                    rider.SetMountedSide(transform);                           //Send the side transform to mount
                    rider.Mount_Side = mountSide;                              //Send the side type to mount
                    rider.OnFindMount.Invoke(transform.root.gameObject);       //Invoke Found Animal
                }
            }
        }
    }
}